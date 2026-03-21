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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ReceitaController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
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
    public async Task<IActionResult> Criar(
        Receita receita,
        IFormFile? fotoReceita,
        string[] ingredientesNomes,
        string[] quantidades,
        string[] passos)
    {
        // Debug da imagem
        if (fotoReceita != null)
            Console.WriteLine($"📸 IMAGEM RECEBIDA: {fotoReceita.FileName} - {fotoReceita.Length} bytes");
        else
            Console.WriteLine("❌ NENHUMA IMAGEM RECEBIDA!");

        // Remove validações desnecessárias
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Erro"] = "Usuário não identificado. Faça login novamente.";
                    return RedirectToAction("Login", "Account");
                }

                receita.UsuarioId = userId;
                receita.Destaque = false;

                // Imagem
                if (fotoReceita != null && fotoReceita.Length > 0)
                {
                    Console.WriteLine("📸 Salvando imagem...");
                    receita.Foto = await SalvarImagemReceita(fotoReceita);
                }
                else
                {
                    receita.Foto = "/img/receitas/default.jpg";
                }

                // Salva receita
                _db.Receitas.Add(receita);
                await _db.SaveChangesAsync();

                Console.WriteLine($"✅ Receita ID: {receita.Id}");

                // Ingredientes
                if (ingredientesNomes != null && quantidades != null)
                {
                    for (int i = 0; i < ingredientesNomes.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(ingredientesNomes[i]) ||
                            string.IsNullOrWhiteSpace(quantidades[i]))
                            continue;

                        var nome = ingredientesNomes[i].Trim().ToLower();

                        var ingredienteExistente = await _db.Ingredientes
                            .FirstOrDefaultAsync(i => i.Nome.ToLower() == nome);

                        int ingredienteId;

                        if (ingredienteExistente != null)
                        {
                            ingredienteId = ingredienteExistente.Id;
                        }
                        else
                        {
                            var novo = new Ingrediente { Nome = ingredientesNomes[i].Trim() };
                            _db.Ingredientes.Add(novo);
                            await _db.SaveChangesAsync();
                            ingredienteId = novo.Id;
                        }

                        _db.ReceitaIngredientes.Add(new ReceitaIngrediente
                        {
                            ReceitaId = receita.Id,
                            IngredienteId = ingredienteId,
                            Quantidade = quantidades[i].Trim()
                        });
                    }
                }

                // Passos
                if (passos != null)
                {
                    foreach (var passo in passos)
                    {
                        if (string.IsNullOrWhiteSpace(passo)) continue;

                        _db.Preparos.Add(new Preparo
                        {
                            ReceitaId = receita.Id,
                            Texto = passo.Trim()
                        });
                    }
                }

                await _db.SaveChangesAsync();

                TempData["Mensagem"] = "Receita criada com sucesso! 🎉";
                return RedirectToAction("Receita", "Home", new { id = receita.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERRO: {ex.Message}");
                TempData["Erro"] = $"Erro ao salvar: {ex.Message}";
            }
        }

        ViewBag.Categorias = _db.Categorias.ToList();
        return View(receita);
    }

    // SALVAR IMAGEM
    private async Task<string> SalvarImagemReceita(IFormFile foto)
    {
        var extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();
        var permitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        if (!permitidas.Contains(extensao))
            throw new Exception("Formato de imagem não permitido.");

        var nome = $"{Guid.NewGuid()}{extensao}";
        var pasta = Path.Combine(_webHostEnvironment.WebRootPath, "img", "receitas");

        if (!Directory.Exists(pasta))
            Directory.CreateDirectory(pasta);

        var caminho = Path.Combine(pasta, nome);

        using var stream = new FileStream(caminho, FileMode.Create);
        await foto.CopyToAsync(stream);

        return $"/img/receitas/{nome}";
    }

    // Minhas receitas
    public async Task<IActionResult> MinhasReceitas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var receitas = await _db.Receitas
            .Where(r => r.UsuarioId == userId)
            .Include(r => r.Categoria)
            .Include(r => r.Ingredientes)
                .ThenInclude(ri => ri.Ingrediente)
            .Include(r => r.Comentarios)
            .Include(r => r.Favoritos)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        return View(receitas);
    }
}