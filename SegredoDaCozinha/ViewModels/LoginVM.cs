using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace SegredoDaCozinha.ViewModels;

public class LoginVM
{
    [Display(Name = "E-mail", Prompt = "Informe seu e-mail ")]
    [Required(ErrorMessage = "O Email é obrigatório!")]
    public string Email { get; set; }

    [Display(Name = "Senha", Prompt = "*****")]
    [Required(ErrorMessage = "A Senha é obrigatória!")]
    [DataType(DataType.Password)]
    public string Senha { get; set; }

    [Display(Name = "Manter Conectado?")]
    public bool Lembrar { get; set; } = false;

    [HiddenInput]
    public string UrlRetorno { get; set; }
}