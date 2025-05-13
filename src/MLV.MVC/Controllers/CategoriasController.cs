using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MLV.Business.Commands;
using MLV.Business.Data.Repository.Interfaces;
using MLV.Business.Services.Interfaces;

namespace MLV.MVC.Controllers;

[Authorize]
public class CategoriasController(ICategoriaService categoriaService, ICategoriaRepository categoriaRepository) : Controller
{
    [AllowAnonymous]
    [HttpGet()]
    public async Task<IActionResult> Index()
    {
        var categorias = await categoriaRepository.ObterTodos();

        return View(categorias);
    }

    [HttpGet()]
    public IActionResult Criar()
    {
        return View();
    }

    [HttpPost()]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(CategoriaRequest request)
    {
        var result = await categoriaService.Adicionar(request);

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
        var categoria = await categoriaRepository.ObterPorId(id);

        if (categoria is null)
            return NotFound();

        return View(new CategoriaRequest() { Id = categoria.Id, Nome = categoria.Nome });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Alterar(Guid id, CategoriaRequest request)
    {
        var categoria = await categoriaRepository.ObterPorId(id);

        if (categoria is null)
            return NotFound();

        request.Id = id;
        var result = await categoriaService.Alterar(request);

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

        var categoria = await categoriaRepository.ObterPorId(id.Value);

        if (categoria is null)
            return NotFound();

        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remover(Guid id)
    {
        var categoria = await categoriaRepository.ObterPorId(id);
        if (categoria is null)
            return NotFound();

        var result = await categoriaService.Remover(id);

        if (!result.IsValid)
        {
            foreach (var erro in result.Errors)
            {
                ModelState.AddModelError("", erro.ErrorMessage);
            }

            return View(nameof(Remover), categoria);
        }

        return RedirectToAction(nameof(Index));
    }
}
