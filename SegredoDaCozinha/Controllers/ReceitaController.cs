using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegredoDaCozinha.Data;
using SegredoDaCozinha.Models;
using System.Security.Claims;

namespace SegredoDaCozinha.Controllers;

[Authorize]
public class ReceitaController : Controller
{
    private readonly AppDbContext _db;

    public ReceitaController(AppDbContext db)
    {
        _db = db;
    }

    // GET: Receita/Criar
    public IActionResult Criar()
    {
        ViewBag.Categorias = _db.Categorias.ToList();
        return View();
    }

    // POST: Receita/Criar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(Receita receita, string[] ingredientesNomes, string[] quantidades, string[] passos)
    {
        // Remove erros de validação
        ModelState.Remove("Usuario");
        ModelState.Remove("UsuarioId");
        ModelState.Remove("Categoria");
        ModelState.Remove("Ingredientes");
        ModelState.Remove("Preparos");
        ModelState.Remove("Comentarios");
        ModelState.Remove("Favoritos");

        if (ModelState.IsValid)
        {
            try
            {
                // Pega o ID do usuário logado
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Erro"] = "Usuário não identificado. Faça login novamente.";
                    return RedirectToAction("Login", "Account");
                }

                receita.UsuarioId = userId;
                receita.Destaque = false;

                // Salva a receita primeiro
                _db.Receitas.Add(receita);
                await _db.SaveChangesAsync();

                // Adiciona os ingredientes
                if (ingredientesNomes != null && quantidades != null)
                {
                    for (int i = 0; i < ingredientesNomes.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(ingredientesNomes[i]) && !string.IsNullOrEmpty(quantidades[i]))
                        {
                            // Verifica se o ingrediente já existe
                            var ingredienteExistente = await _db.Ingredientes
                                .FirstOrDefaultAsync(ing => ing.Nome.ToLower() == ingredientesNomes[i].ToLower().Trim());

                            int ingredienteId;

                            if (ingredienteExistente != null)
                            {
                                ingredienteId = ingredienteExistente.Id;
                            }
                            else
                            {
                                // Cria novo ingrediente
                                var novoIngrediente = new Ingrediente
                                {
                                    Nome = ingredientesNomes[i].Trim()
                                };
                                _db.Ingredientes.Add(novoIngrediente);
                                await _db.SaveChangesAsync();
                                ingredienteId = novoIngrediente.Id;
                            }

                            // Adiciona o relacionamento
                            var receitaIngrediente = new ReceitaIngrediente
                            {
                                ReceitaId = receita.Id,
                                IngredienteId = ingredienteId,
                                Quantidade = quantidades[i].Trim()
                            };
                            _db.ReceitaIngredientes.Add(receitaIngrediente);
                        }
                    }
                }

                // Adiciona os passos de preparo
                if (passos != null)
                {
                    foreach (var passo in passos)
                    {
                        if (!string.IsNullOrEmpty(passo))
                        {
                            var preparo = new Preparo
                            {
                                ReceitaId = receita.Id,
                                Texto = passo.Trim()
                            };
                            _db.Preparos.Add(preparo);
                        }
                    }
                }

                await _db.SaveChangesAsync();

                TempData["Mensagem"] = "Receita criada com sucesso! 🎉";
                return RedirectToAction("Receita", "Home", new { id = receita.Id });
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao salvar: {ex.Message}";
            }
        }

        ViewBag.Categorias = _db.Categorias.ToList();
        return View(receita);
    }

    // GET: Receita/MinhasReceitas
    public async Task<IActionResult> MinhasReceitas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var minhasReceitas = await _db.Receitas
            .Where(r => r.UsuarioId == userId)
            .Include(r => r.Categoria)
            .Include(r => r.Ingredientes)
            .ThenInclude(ri => ri.Ingrediente)
            .Include(r => r.Comentarios)
            .Include(r => r.Favoritos)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        return View(minhasReceitas);
    }
}