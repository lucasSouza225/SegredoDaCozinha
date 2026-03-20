using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SegredoDaCozinha.Data;
using SegredoDaCozinha.Models;
using Microsoft.EntityFrameworkCore;

namespace SegredoDaCozinha.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    // ===== CATEGORIAS =====
    public IActionResult Categorias()
    {
        var categorias = _db.Categorias.ToList();
        return View(categorias);
    }

    [HttpGet]
    public IActionResult CriarCategoria()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CriarCategoria(Categoria categoria)
    {
        if (ModelState.IsValid)
        {
            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();
            return RedirectToAction("Categorias");
        }
        return View(categoria);
    }

    // ===== INGREDIENTES =====
    public IActionResult Ingredientes()
    {
        var ingredientes = _db.Ingredientes.ToList();
        return View(ingredientes);
    }

    [HttpGet]
    public IActionResult CriarIngrediente()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CriarIngrediente(Ingrediente ingrediente)
    {
        if (ModelState.IsValid)
        {
            _db.Ingredientes.Add(ingrediente);
            await _db.SaveChangesAsync();
            return RedirectToAction("Ingredientes");
        }
        return View(ingrediente);
    }

    // ===== RECEITAS =====
    public IActionResult Receitas()
    {
        var receitas = _db.Receitas
            .Include(r => r.Categoria)
            .Include(r => r.Usuario)
            .ToList();
        return View(receitas);
    }

    [HttpGet]
    public IActionResult CriarReceita()
    {
        ViewBag.Categorias = _db.Categorias.ToList();
        ViewBag.Ingredientes = _db.Ingredientes.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CriarReceita(Receita receita, int[] ingredientesIds, string[] quantidades)
    {
        if (ModelState.IsValid)
        {
            _db.Receitas.Add(receita);
            await _db.SaveChangesAsync();

            // Adicionar ingredientes
            for (int i = 0; i < ingredientesIds.Length; i++)
            {
                if (ingredientesIds[i] > 0 && !string.IsNullOrEmpty(quantidades[i]))
                {
                    var receitaIngrediente = new ReceitaIngrediente
                    {
                        ReceitaId = receita.Id,
                        IngredienteId = ingredientesIds[i],
                        Quantidade = quantidades[i]
                    };
                    _db.ReceitaIngredientes.Add(receitaIngrediente);
                }
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Receitas");
        }
        return View(receita);
    }
}