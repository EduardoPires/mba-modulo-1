using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MLV.Business.Data.Repository.Interfaces;
using MLV.Business.Models;
using MLV.MVC.Models;
using System.Diagnostics;

namespace MLV.MVC.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository) : Controller
    {
        public async Task<IActionResult> Index(Guid? categoriaId)
        {
            logger.LogInformation("Listando os produtos");
            var categorias = await categoriaRepository.ObterTodos();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nome");
            var produtos = await produtoRepository.ObterTodos(categoriaId);
            return View(produtos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
