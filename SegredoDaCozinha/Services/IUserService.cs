using SegredoDaCozinha.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SegredoDaCozinha.Services;

public interface IUsuarioService
{
    Task<UsuarioVM> GetUsuarioLogado();
    Task<SignInResult> LoginUsuario(LoginVM login);
    Task LogoutUsuario();
}