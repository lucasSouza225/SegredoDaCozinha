using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SegredoDaCozinha.Models;

[Table("Receitas")]
public class Receita
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "A Categoria é obrigatória")]
    public int CategoriaId { get; set; }
    [ForeignKey(nameof(CategoriaId))]
    public Categoria Categoria { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; }

    [StringLength(1000)]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; }

    [StringLength(30)]
    [Display(Name = "Tempo de Preparo")]
    public string TempoPreparo { get; set; }

    public int Rendimento { get; set; }

    public Dificuldade Dificuldade { get; set; }

    [StringLength(300)]
    public string Foto { get; set; }

    public bool Destaque { get; set; } = false;

    public string DicaDoChef { get; set; }

    [Required]
    public string UsuarioId { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; }

    // Propriedades de navegação
    public List<Preparo> Preparos { get; set; }
    public List<ReceitaIngrediente> Ingredientes { get; set; }
    public List<Favorito> Favoritos { get; set; }
    public List<Comentario> Comentarios { get; set; }
}