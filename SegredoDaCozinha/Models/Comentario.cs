using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegredoDaCozinha.Models;

[Table("Comentarios")]
public class Comentario
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ReceitaId { get; set; }
    [ForeignKey(nameof(ReceitaId))]
    public Receita Receita { get; set; }

    [Required]
    public string UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }

    [Required]
    [StringLength(1000)]
    public string TextoComentario { get; set; }

    public DateTime DataComentario { get; set; } = DateTime.Now;
}