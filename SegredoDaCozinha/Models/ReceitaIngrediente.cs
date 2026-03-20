using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegredoDaCozinha.Models;

[Table("ReceitaIngredientes")]
public class ReceitaIngrediente
{
    [Key, Column(Order = 1)]
    public int ReceitaId { get; set; }
    [ForeignKey(nameof(ReceitaId))]
    public Receita Receita { get; set; }

    [Key, Column(Order = 2)]
    public int IngredienteId { get; set; }
    [ForeignKey(nameof(IngredienteId))]
    public Ingrediente Ingrediente { get; set; }

    [Required]
    [StringLength(30)]
    public string Quantidade { get; set; }
}