using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegredoDaCozinha.Models;

[Table("Favoritos")]
public class Favorito
{
    [Key, Column(Order = 1)]
    public int ReceitaId { get; set; }
    [ForeignKey(nameof(ReceitaId))]
    public Receita Receita { get; set; }

    [Key, Column(Order = 2)]
    public string UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }
}