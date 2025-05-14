using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MLV.Business.Commands;
using MLV.Business.Data.Repository.Interfaces;
using MLV.Business.Services.Interfaces;
using NuGet.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MLV.MVC.Controllers;

[Authorize]
public class ProdutosController(IProdutoService produtoService, IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository, IWebHostEnvironment webHostEnvironment) : Controller
{
    [HttpGet()]
    public async Task<IActionResult> Index()
    {
        var produtos = await produtoService.ObterProdutosPorVendedorId();

        return View(produtos);
    }

    [HttpGet()]
    public async Task<IActionResult> Criar()
    {
        ViewBag.Categorias = new SelectList(await categoriaRepository.ObterTodos(), "Id", "Nome");
        return View();
    }

    [HttpPost()]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(ProdutoRequest request)
    {
        request.WebRootPath = webHostEnvironment.WebRootPath;
        ViewBag.Categorias = new SelectList(await categoriaRepository.ObterTodos(), "Id", "Nome");

        var result = await produtoService.Adicionar(request);

        if (!result.IsValid)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.ErrorMessage);
            }
            return View(nameof(Criar), request);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet()]
    public async Task<IActionResult> Alterar(Guid id)
    {
        var produto = await produtoRepository.ObterPorId(id);

        if (produto is null)
            return NotFound();

        var listaCategorias = new List<SelectListItem>();
        foreach (var item in await categoriaRepository.ObterTodos())
        {
            listaCategorias.Add(new SelectListItem(item.Nome, item.Id.ToString(), produto.Id == item.Id));
        }

        ViewBag.Categorias = listaCategorias;

        return View(new ProdutoRequestAlteracao() { 
            Id = produto.Id, 
            Nome = produto.Nome,
            CategoriaId = produto.CategoriaId,
            Descricao = produto.Descricao,
            Estoque = produto.Estoque,
            Valor = produto.Valor,
            CaminhoImagem = produto.CaminhoImagem
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Alterar(Guid id, ProdutoRequestAlteracao request)
    {
        var produto = await produtoRepository.ObterPorId(id);

        if (produto is null)
            return NotFound();

        request.Id = id;
        var result = await produtoService.Alterar(request);

        if (!result.IsValid)
        {
            foreach (var erro in result.Errors)
            {
                ModelState.AddModelError("", erro.ErrorMessage);
            }

            return View(nameof(Alterar), request);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Remover(Guid? id)
    {
        if (id is null)
            return NotFound();

        var produto = await produtoRepository.ObterPorId(id.Value);

        if (produto is null)
            return NotFound();

        return View(produto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remover(Guid id)
    {
        var produto = await produtoRepository.ObterPorId(id);
        if (produto is null)
            return NotFound();

        var result = await produtoService.Remover(id);

        if (!result.IsValid)
        {
            foreach (var erro in result.Errors)
            {
                ModelState.AddModelError("", erro.ErrorMessage);
            }

            return View(nameof(Remover), produto);
        }

        return RedirectToAction(nameof(Index));
    }
}
