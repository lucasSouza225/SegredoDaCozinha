using SegredoDaCozinha.Services;
using SegredoDaCozinha.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SegredoDaCozinha.Controllers;

public class AccountController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IUsuarioService usuarioService,
        ILogger<AccountController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        // Se o usuário já estiver autenticado, redireciona para a home
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new LoginVM
        {
            UrlRetorno = returnUrl ?? Url.Content("~/")
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginVM model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { 
                    success = false, 
                    message = "Dados inválidos. Verifique os campos preenchidos." 
                });
            }

            var result = await _usuarioService.LoginUsuario(model);

            if (result.Succeeded)
            {
                return Json(new { 
                    success = true, 
                    message = "Login realizado com sucesso! Redirecionando...",
                    redirectUrl = model.UrlRetorno ?? Url.Action("Index", "Home")
                });
            }

            if (result.IsLockedOut)
            {
                return Json(new { 
                    success = false, 
                    message = "Usuário bloqueado por muitas tentativas. Tente novamente mais tarde." 
                });
            }

            if (result.IsNotAllowed)
            {
                return Json(new { 
                    success = false, 
                    message = "Usuário não tem permissão para acessar o sistema." 
                });
            }

            return Json(new { 
                success = false, 
                message = "E-mail ou senha incorretos. Tente novamente." 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login");
            return Json(new { 
                success = false, 
                message = "Ocorreu um erro interno. Tente novamente mais tarde." 
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _usuarioService.LogoutUsuario();
        return RedirectToAction("Index", "Home");
    }
}