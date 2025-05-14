using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MLV.Business.Commands;
using MLV.Business.Data.Repository.Interfaces;
using MLV.Business.Models;
using MLV.Business.Services.Interfaces;

namespace MLV.Business.Services;
public class CategoriaService : ServiceHandler, ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly string _usuarioLogado;
    public CategoriaService(ICategoriaRepository categoriaRepository, IProdutoRepository produtoRepository, IHttpContextAccessor httpContextAccessor)
    {
        _categoriaRepository = categoriaRepository;
        _produtoRepository = produtoRepository;

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null && httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
        {
            _usuarioLogado = httpContext.User.Identity.Name;
        }
    }

    public async Task<ValidationResult> Adicionar(CategoriaRequest request)
    {
        if (!request.IsValid()) return request.ValidationResult;

        var categoria = new Categoria(request.Id, request.Nome, request.Descricao, _usuarioLogado);

        if (!await ValidarCategoriaUnicaPorNome(categoria.Nome))
        {
            AdicionarErro("Já existe uma categoria com esse nome.");
            return ValidationResult;
        }

        _categoriaRepository.Adicionar(categoria);

        return await PersistirDados(_categoriaRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Alterar(CategoriaRequest request)
    {
        if (!request.IsValid()) return request.ValidationResult;

        var categoria = await _categoriaRepository.ObterPorId(request.Id);

        if (categoria is null)
        {
            AdicionarErro("A categoria não foi encontrada.");
            return ValidationResult;
        }

        categoria.Atualizar(request.Nome, request.Descricao, _usuarioLogado);

        if (!await ValidarCategoriaUnicaPorNome(categoria.Nome, categoria.Id))
        {
            AdicionarErro("Já existe uma categoria com esse nome.");
            return ValidationResult;
        }

        _categoriaRepository.Atualizar(categoria);

        return await PersistirDados(_categoriaRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Remover(Guid id)
    {
        if (id == Guid.Empty)
        {
            AdicionarErro("O id precisa ser informado.");
        }

        var categoria = await _categoriaRepository.ObterPorId(id);

        if (categoria is null)
        {
            AdicionarErro("O categoria não foi encontrado.");
            return ValidationResult;
        }

        if (!await ValidarSeNaoTemProdutos(id))
        {
            AdicionarErro("Não é possível remover esta categoria, pois existem produtos associados.");
            return ValidationResult;
        }

        _categoriaRepository.Remover(categoria);

        return await PersistirDados(_categoriaRepository.UnitOfWork);
    }

    private async Task<bool> ValidarCategoriaUnicaPorNome(string nome, Guid? id = null)
    {
        var categoria = await _categoriaRepository.ObterPorNome(nome);

        if (categoria is null)
            return true;

        if (id.HasValue && categoria.Id == id.Value)
            return true;

        return false;
    }

    private async Task<bool> ValidarSeNaoTemProdutos(Guid id)
    {
        var produtos = await _produtoRepository.ObterPorCategoriaId(id);

        return produtos.Count == 0;
    }
}
