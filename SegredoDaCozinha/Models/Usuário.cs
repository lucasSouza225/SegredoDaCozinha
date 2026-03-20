using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SegredoDaCozinha.Models;

public class Usuario : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? DataNascimento { get; set; }
    
    [StringLength(300)]
    public string Foto { get; set; }
}