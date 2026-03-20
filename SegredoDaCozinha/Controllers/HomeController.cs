using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SegredoDaCozinha.Models;
using SegredoDaCozinha.Data;
using Microsoft.EntityFrameworkCore;

namespace SegredoDaCozinha.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDb)
    {
        _logger = logger;
        _db = appDb;
    }

    public IActionResult Index()
    {
        var receitas = _db.Receitas
            .Include(r => r.Ingredientes)
            .Include(r => r.Categoria)
            .Include(r => r.Comentarios)
            .Include(r => r.Favoritos)
            .ToList();
        ViewData["Categorias"] = _db.Categorias.ToList();
        ViewData["Destaque"] = _db.Receitas.FirstOrDefault(r => r.Destaque);
        return View(receitas);
    }

    public IActionResult Receita(int id)
    {
        var receita = _db.Receitas
            .Where(r => r.Id == id)
            .Include(r => r.Categoria)
            .Include(r => r.Preparos)
            .Include(r => r.Ingredientes)
            .ThenInclude(ri => ri.Ingrediente)
            .Include(r => r.Comentarios)
            .ThenInclude(c => c.Usuario)
            .Include(r => r.Favoritos)
            .FirstOrDefault();
        return View(receita);
    }

    public IActionResult Receitas()
    {
        return View();
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