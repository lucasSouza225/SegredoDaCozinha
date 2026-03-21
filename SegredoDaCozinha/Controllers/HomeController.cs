using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SegredoDaCozinha.Models;
using SegredoDaCozinha.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        
        if (receita == null)
        {
            return NotFound();
        }
        
        return View(receita);
    }

    public IActionResult Receitas()
    {
        var receitas = _db.Receitas
            .Include(r => r.Ingredientes)
            .Include(r => r.Categoria)
            .Include(r => r.Comentarios)
            .Include(r => r.Favoritos)
            .ToList();
        
        ViewBag.Categorias = _db.Categorias.ToList();
        return View(receitas);
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

    // ===== EDITAR RECEITA =====
    [Authorize]
    public async Task<IActionResult> Editar(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var receita = await _db.Receitas
            .Include(r => r.Ingredientes)
                .ThenInclude(ri => ri.Ingrediente)
            .Include(r => r.Preparos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receita == null)
        {
            return NotFound();
        }

        if (receita.UsuarioId != userId)
        {
            TempData["Erro"] = "Você não tem permissão para editar esta receita.";
            return RedirectToAction("Receita", new { id });
        }

        ViewBag.Categorias = await _db.Categorias.ToListAsync();
        return View(receita);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Receita receita, 
        string[] ingredientesNomes, string[] quantidades, string[] passos)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var receitaOriginal = await _db.Receitas
            .Include(r => r.Ingredientes)
            .Include(r => r.Preparos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receitaOriginal == null)
        {
            return NotFound();
        }

        if (receitaOriginal.UsuarioId != userId)
        {
            TempData["Erro"] = "Você não tem permissão para editar esta receita.";
            return RedirectToAction("Receita", new { id });
        }

        ModelState.Remove("Usuario");
        ModelState.Remove("UsuarioId");
        ModelState.Remove("Categoria");
        ModelState.Remove("Ingredientes");
        ModelState.Remove("Preparos");

        if (ModelState.IsValid)
        {
            receitaOriginal.Nome = receita.Nome;
            receitaOriginal.Descricao = receita.Descricao;
            receitaOriginal.CategoriaId = receita.CategoriaId;
            receitaOriginal.Dificuldade = receita.Dificuldade;
            receitaOriginal.Rendimento = receita.Rendimento;
            receitaOriginal.TempoPreparo = receita.TempoPreparo;
            receitaOriginal.Foto = receita.Foto;
            receitaOriginal.DicaDoChef = receita.DicaDoChef;

            _db.ReceitaIngredientes.RemoveRange(receitaOriginal.Ingredientes);
            _db.Preparos.RemoveRange(receitaOriginal.Preparos);

            if (ingredientesNomes != null && quantidades != null)
            {
                for (int i = 0; i < ingredientesNomes.Length; i++)
                {
                    if (!string.IsNullOrEmpty(ingredientesNomes[i]) && !string.IsNullOrEmpty(quantidades[i]))
                    {
                        var ingredienteExistente = await _db.Ingredientes
                            .FirstOrDefaultAsync(ing => ing.Nome.ToLower() == ingredientesNomes[i].ToLower().Trim());

                        int ingredienteId;

                        if (ingredienteExistente != null)
                        {
                            ingredienteId = ingredienteExistente.Id;
                        }
                        else
                        {
                            var novoIngrediente = new Ingrediente
                            {
                                Nome = ingredientesNomes[i].Trim()
                            };
                            _db.Ingredientes.Add(novoIngrediente);
                            await _db.SaveChangesAsync();
                            ingredienteId = novoIngrediente.Id;
                        }

                        var receitaIngrediente = new ReceitaIngrediente
                        {
                            ReceitaId = receitaOriginal.Id,
                            IngredienteId = ingredienteId,
                            Quantidade = quantidades[i].Trim()
                        };
                        _db.ReceitaIngredientes.Add(receitaIngrediente);
                    }
                }
            }

            if (passos != null)
            {
                foreach (var passo in passos)
                {
                    if (!string.IsNullOrEmpty(passo))
                    {
                        var preparo = new Preparo
                        {
                            ReceitaId = receitaOriginal.Id,
                            Texto = passo.Trim()
                        };
                        _db.Preparos.Add(preparo);
                    }
                }
            }

            await _db.SaveChangesAsync();
            TempData["Mensagem"] = "Receita atualizada com sucesso!";
            return RedirectToAction("Receita", new { id = receitaOriginal.Id });
        }

        ViewBag.Categorias = await _db.Categorias.ToListAsync();
        return View(receita);
    }

    // ===== DELETAR RECEITA =====
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deletar(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var receita = await _db.Receitas
            .Include(r => r.Ingredientes)
            .Include(r => r.Preparos)
            .Include(r => r.Comentarios)
            .Include(r => r.Favoritos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receita == null)
        {
            return NotFound();
        }

        if (receita.UsuarioId != userId)
        {
            TempData["Erro"] = "Você não tem permissão para deletar esta receita.";
            return RedirectToAction("Receita", new { id });
        }

        _db.ReceitaIngredientes.RemoveRange(receita.Ingredientes);
        _db.Preparos.RemoveRange(receita.Preparos);
        _db.Comentarios.RemoveRange(receita.Comentarios);
        _db.Favoritos.RemoveRange(receita.Favoritos);
        _db.Receitas.Remove(receita);
        await _db.SaveChangesAsync();

        TempData["Mensagem"] = "Receita deletada com sucesso!";
        return RedirectToAction("Index");
    }
}