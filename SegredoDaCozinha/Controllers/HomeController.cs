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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDb, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _db = appDb;
        _webHostEnvironment = webHostEnvironment;
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
            return NotFound();

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

    // ===== EDITAR (GET) =====
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
            return NotFound();

        if (receita.UsuarioId != userId)
        {
            TempData["Erro"] = "Você não tem permissão para editar esta receita.";
            return RedirectToAction("Receita", new { id });
        }

        ViewBag.Categorias = await _db.Categorias.ToListAsync();
        return View(receita);
    }

    // ===== EDITAR (POST) =====
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        Receita receita,
        IFormFile? fotoReceita,
        string[] ingredientesNomes,
        string[] quantidades,
        string[] passos)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var receitaOriginal = await _db.Receitas
            .Include(r => r.Ingredientes)
            .Include(r => r.Preparos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receitaOriginal == null)
            return NotFound();

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
            try
            {
                // Atualiza dados
                receitaOriginal.Nome = receita.Nome;
                receitaOriginal.Descricao = receita.Descricao;
                receitaOriginal.CategoriaId = receita.CategoriaId;
                receitaOriginal.Dificuldade = receita.Dificuldade;
                receitaOriginal.Rendimento = receita.Rendimento;
                receitaOriginal.TempoPreparo = receita.TempoPreparo;
                receitaOriginal.DicaDoChef = receita.DicaDoChef;

                // Foto
                if (fotoReceita != null && fotoReceita.Length > 0)
                {
                    Console.WriteLine($"📸 Nova imagem: {fotoReceita.FileName}");

                    if (!string.IsNullOrEmpty(receitaOriginal.Foto) &&
                        receitaOriginal.Foto != "/img/receitas/default.jpg")
                    {
                        var caminhoAntigo = Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            receitaOriginal.Foto.TrimStart('/')
                        );

                        if (System.IO.File.Exists(caminhoAntigo))
                        {
                            System.IO.File.Delete(caminhoAntigo);
                            Console.WriteLine($"🗑️ Foto antiga removida");
                        }
                    }

                    receitaOriginal.Foto = await SalvarImagemReceita(fotoReceita);
                }

                // Limpa antigos
                _db.ReceitaIngredientes.RemoveRange(receitaOriginal.Ingredientes);
                _db.Preparos.RemoveRange(receitaOriginal.Preparos);

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
                            ReceitaId = receitaOriginal.Id,
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
                            ReceitaId = receitaOriginal.Id,
                            Texto = passo.Trim()
                        });
                    }
                }

                await _db.SaveChangesAsync();

                TempData["Mensagem"] = "Receita atualizada com sucesso!";
                return RedirectToAction("Receita", new { id = receitaOriginal.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro: {ex.Message}");
                TempData["Erro"] = $"Erro ao salvar: {ex.Message}";
            }
        }

        ViewBag.Categorias = await _db.Categorias.ToListAsync();
        return View(receita);
    }

    // ===== DELETAR =====
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
            return NotFound();

        if (receita.UsuarioId != userId)
        {
            TempData["Erro"] = "Você não tem permissão para deletar.";
            return RedirectToAction("Receita", new { id });
        }

        if (!string.IsNullOrEmpty(receita.Foto) &&
            receita.Foto != "/img/receitas/default.jpg")
        {
            var caminho = Path.Combine(_webHostEnvironment.WebRootPath, receita.Foto.TrimStart('/'));

            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);
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

    // ===== SALVAR IMAGEM =====
    private async Task<string> SalvarImagemReceita(IFormFile foto)
    {
        var ext = Path.GetExtension(foto.FileName).ToLowerInvariant();
        var permitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        if (!permitidas.Contains(ext))
            throw new Exception("Formato inválido");

        var nome = $"{Guid.NewGuid()}{ext}";
        var pasta = Path.Combine(_webHostEnvironment.WebRootPath, "img", "receitas");

        if (!Directory.Exists(pasta))
            Directory.CreateDirectory(pasta);

        var caminho = Path.Combine(pasta, nome);

        using var stream = new FileStream(caminho, FileMode.Create);
        await foto.CopyToAsync(stream);

        return $"/img/receitas/{nome}";
    }
}