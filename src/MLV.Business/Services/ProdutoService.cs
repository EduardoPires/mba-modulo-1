using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MLV.Business.Commands;
using MLV.Business.Data.Repository.Interfaces;
using MLV.Business.Models;
using MLV.Business.Services.Interfaces;
using System.Security.Claims;

namespace MLV.Business.Services;

public class ProdutoService : ServiceHandler, IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly Guid _userId;
    private readonly string _usuarioLogado;
    private readonly string _diretorioBase = "uploads";

    public ProdutoService(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository, IHttpContextAccessor httpContextAccessor)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null && httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
        {
            _userId = Guid.Parse(httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            _usuarioLogado = httpContext.User.Identity.Name;
        }
    }

    public async Task<IEnumerable<Produto>> ObterProdutosPorVendedorId()
    {
        var produtos = await _produtoRepository.ObterPorVendedorId(_userId);
        if (produtos is null)
        {
            return [];
        }
        return produtos;
    }

    public async Task<ValidationResult> Adicionar(ProdutoRequest request)
    {
        if (!request.IsValid()) return request.ValidationResult;

        var caminhoImagem = await ObterCaminhoDaImagem(request.Imagem, request.WebRootPath);

        if (string.IsNullOrWhiteSpace(caminhoImagem))
            return ValidationResult;

        var produto = new Produto(request.Id, request.CategoriaId, _userId, request.Nome, request.Descricao, request.Valor, request.Estoque, caminhoImagem, _usuarioLogado);

        if (!await ValidarCategoriaExiste(produto.CategoriaId))
        {
            AdicionarErro("Categoria não foi encontrada.");
            return ValidationResult;
        }

        _produtoRepository.Adicionar(produto);

        return await PersistirDados(_produtoRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Alterar(ProdutoRequestAlteracao request)
    {
        if (!request.IsValid()) return request.ValidationResult;

        var produto = await _produtoRepository.ObterPorId(request.Id);

        if (produto is null)
        {
            AdicionarErro("O produto não foi encontrado.");
            return ValidationResult;
        }

        produto.AtualizarProduto(request.CategoriaId, request.Nome, request.Descricao, request.Valor, request.Estoque, _usuarioLogado);

        if (request.Imagem != null && request.Imagem.Length > 0)
        {
            RemoverImagem(request, produto);
            var caminhoImagem = await ObterCaminhoDaImagem(request.Imagem, request.WebRootPath);
            produto.AtualizarImagem(caminhoImagem);
        }

        if (!await ValidarCategoriaExiste(produto.CategoriaId))
        {
            AdicionarErro("A categoria não foi encontrada.");
            return ValidationResult;
        }

        if (!await ValidarProdutoPertenceAoVendedor(produto.Id))
        {
            AdicionarErro("Este produto não pertence ao seu catálogo");
            return ValidationResult;
        }

        _produtoRepository.Atualizar(produto);

        return await PersistirDados(_produtoRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Remover(Guid id)
    {
        if (id == Guid.Empty)
        {
            AdicionarErro("O id precisa ser informado.");
        }

        var produto = await _produtoRepository.ObterPorId(id);

        if (produto is null)
        {
            AdicionarErro("O produto não foi encontrado.");
            return ValidationResult;
        }

        if (!await ValidarProdutoPertenceAoVendedor(id))
        {
            AdicionarErro("Este produto não pertence ao seu catálogo");
            return ValidationResult;
        }

        _produtoRepository.Remover(produto);

        return await PersistirDados(_produtoRepository.UnitOfWork);
    }

    private async Task<bool> ValidarCategoriaExiste(Guid categoriaId)
    {
        var categoria = await _categoriaRepository.ObterPorId(categoriaId);

        return categoria is not null;
    }

    private async Task<bool> ValidarProdutoPertenceAoVendedor(Guid id)
    {
        var produto = await _produtoRepository.ObterPorVendedorId(_userId);

        return produto is not null && produto.FirstOrDefault(p => p.Id == id) is not null;
    }

    private async Task<string> ObterCaminhoDaImagem(IFormFile arquivo, string webRootPath)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            AdicionarErro("Nenhum arquivo foi enviado.");
            return string.Empty;
        }

        string extensao = Path.GetExtension(arquivo.FileName);
        string[] extensoesPermitidas = [".jpg", ".jpeg", ".png", ".gif"];
        if (!Array.Exists(extensoesPermitidas, e => e.Equals(extensao, StringComparison.OrdinalIgnoreCase)))
        {
            AdicionarErro($"Extensão de arquivo não permitida: {extensao}");
            return string.Empty;
        }

        string nomeArquivo = $"{Guid.NewGuid()}{extensao}";

        var uploadsFolder = Path.Combine(webRootPath, _diretorioBase);
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, nomeArquivo);

        await SalvarImagem(filePath, arquivo);

        return $"/{_diretorioBase}/{nomeArquivo}";
    }

    private static async Task SalvarImagem(string caminhoCompleto, IFormFile arquivo)
    {
        using var stream = new FileStream(caminhoCompleto, FileMode.Create);
        await arquivo.CopyToAsync(stream);
    }

    private void RemoverImagem(ProdutoRequestAlteracao request, Produto produto)
    {
        var caminhoImagemArray = produto.CaminhoImagem.Split("/");
        var imagemId = caminhoImagemArray[^1];
        File.Delete($"{request.WebRootPath}/{_diretorioBase}/{imagemId}");
    }
}
