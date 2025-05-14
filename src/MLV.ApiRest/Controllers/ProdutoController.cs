using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MLV.ApiRest.Api;
using MLV.Business.Commands;
using MLV.Business.Data.Repository.Interfaces;
using MLV.Business.Models;
using MLV.Business.Services;
using MLV.Business.Services.Interfaces;

namespace MLV.ApiRest.Controllers;

[Authorize]
[Route("api/produtos")]
public class ProdutoController(IProdutoRepository produtoRepository,
                      IProdutoService produtoService) : MainController
{
    private const string _caminhoImagemMvc = "..\\MLV.MVC\\wwwroot";

    [AllowAnonymous]
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Produto>))]
    public async Task<ActionResult> ObterTodos(Guid? categoriaId)
    {
        var produtos = await produtoRepository.ObterTodos(categoriaId);

        return CustomResponse(produtos);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Categoria))]
    public async Task<ActionResult> ObterPorId(Guid id)
    {
        var produto = await produtoRepository.ObterPorId(id);

        return CustomResponse(produto);
    }

    [HttpGet("por-vendedor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Produto>))]
    public async Task<ActionResult> ObterTodosPorVendedor()
    {
        var produtos = await produtoService.ObterProdutosPorVendedorId();

        return CustomResponse(produtos);
    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<ActionResult> Criar(ProdutoRequest request)
    {
        request.WebRootPath = Path.GetFullPath(_caminhoImagemMvc);

        var result = await produtoService.Adicionar(request);

        if (!result.IsValid)
            return CustomResponse(result);

        return CreatedAtAction(nameof(ObterPorId), new { id = request.Id }, null);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<ActionResult> Alterar(Guid id, ProdutoRequestAlteracao request)
    {
        var produto = await produtoRepository.ObterPorId(id);

        if (produto is null)
            return NotFound();

        request.Id = id;
        request.WebRootPath = Path.GetFullPath(_caminhoImagemMvc);
        var result = await produtoService.Alterar(request);

        if (!result.IsValid)
            return CustomResponse(result);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<ActionResult> Remover(Guid id)
    {
        var produto = await produtoRepository.ObterPorId(id);

        if (produto is null)
            return NotFound();

        var result = await produtoService.Remover(id);

        if (!result.IsValid)
            return CustomResponse(result);

        return NoContent();
    }
}
