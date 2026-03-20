using System.Security.Claims;
using SegredoDaCozinha.Data;
using SegredoDaCozinha.Helpers;
using SegredoDaCozinha.Models;
using SegredoDaCozinha.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SegredoDaCozinha.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        AppDbContext context,
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UsuarioService> logger
    )
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<UsuarioVM> GetUsuarioLogado()
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return null;
        }
        var userAccount = await _userManager.FindByIdAsync(userId);
        var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Id == userId);
        var perfis = string.Join(", ", await _userManager.GetRolesAsync(userAccount));
        var isAdmin = await _userManager.IsInRoleAsync(userAccount, "Administrador");
        UsuarioVM usuarioVM = new()
        {
            UsuarioId = userId,
            Nome = usuario.Nome,
            DataNascimento = usuario.DataNascimento,
            Foto = usuario.Foto,
            Email = userAccount.Email,
            UserName = userAccount.UserName,
            Perfil = perfis,
            IsAdmin = isAdmin
        };
        return usuarioVM;
    }
    
    public async Task<SignInResult> LoginUsuario(LoginVM login)
    {
        string userName = login.Email;
        if (Helper.IsValidEmail(login.Email))
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user != null)
                userName = user.UserName;
        }

        var result = await _signInManager.PasswordSignInAsync(
            userName, login.Senha, login.Lembrar, lockoutOnFailure: true
        );

        if (result.Succeeded)
            _logger.LogInformation($"Usuário '{userName}' acessou o sistema");
        if (result.IsLockedOut)
            _logger.LogWarning($"Usuário '{userName}' está bloqueado");

        return result;
    }

    public async Task LogoutUsuario()
    {
        _logger.LogInformation($"Usuário fez logoff");
        await _signInManager.SignOutAsync();
    }
}