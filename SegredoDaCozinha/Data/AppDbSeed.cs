using SegredoDaCozinha.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SegredoDaCozinha.Data;

public class AppDbSeed
{
    public AppDbSeed(ModelBuilder builder)
    {
        #region Popular Perfil
        List<IdentityRole> roles = new()
        {
            new()
            {
                Id = "36f25c22-38d8-4be6-af11-5a709cf16bc9",
                Name = "Administrador",
                NormalizedName = "ADMINISTRADOR"
            },
            new()
            {
                Id = "a20c3739-5650-4cab-8a8f-d6cec2b151be",
                Name = "Moderador",
                NormalizedName = "MODERADOR"
            },
            new()
            {
                Id = "1ce68b46-785b-4d5e-9d5e-525554915d46",
                Name = "Usuário",
                NormalizedName = "USUÁRIO"
            },
        };
        builder.Entity<IdentityRole>().HasData(roles);
        #endregion
    
        #region Popular Usuários
        List<Usuario> usuarios = new()
        {
            new()
            {
                Id = "36f272b6-fbde-41b5-9b85-2d3661567ccf",
                Nome = "ADMINISTRADOR",
                UserName = "administrador@SegredoDaCozinha.com",
                NormalizedUserName = "ADMINISTRADOR@SegredoDaCozinha.COM",
                Email = "administrador@SegredoDaCozinha.com",
                NormalizedEmail = "ADMINISTRADOR@SegredoDaCozinha.COM",
                DataNascimento = DateTime.Parse("05/08/1981"),
                Foto = "/img/usuarios/36f272b6-fbde-41b5-9b85-2d3661567ccf.png",
                EmailConfirmed = true,
                LockoutEnabled = false
            },
            new()
            {
                Id = "0cbf5d2b-b2d5-4b06-bb4d-3b353b10e2b0",
                Nome = "José Antonio Gallo Junior",
                UserName = "gallojunior@gmail.com",
                NormalizedUserName = "GALLOJUNIOR@GMAIL.COM",
                Email = "gallojunior@gmail.com",
                NormalizedEmail = "GALLOJUNIOR@GMAIL.COM",
                DataNascimento = DateTime.Parse("05/08/1981"),
                Foto = "/img/usuarios/0cbf5d2b-b2d5-4b06-bb4d-3b353b10e2b0.png",
                EmailConfirmed = true,
                LockoutEnabled = true
            },
        };
        foreach (var usuario in usuarios)
        {
            PasswordHasher<IdentityUser> passwordHasher = new();
            usuario.PasswordHash = passwordHasher.HashPassword(usuario, "123456");
        }
        builder.Entity<Usuario>().HasData(usuarios);
        #endregion
    
        #region Popular Perfil Usuário
        List<IdentityUserRole<string>> userRoles = new()
        {
            new()
            {
                UserId = usuarios[0].Id,
                RoleId = roles[0].Id
            },
            new()
            {
                UserId = usuarios[1].Id,
                RoleId = roles[2].Id
            }
        };
        builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        #endregion
    
        #region Popular Categoria
        List<Categoria> categorias = new()
        {
            new()
            {
                Id = 1,
                Nome = "Massas",
                Icone = "fas fa-pizza-slice",
            },
            new()
            {
                Id = 2,
                Nome = "Peixes",
                Icone = "fas fa-fish",
            },
            new()
            {
                Id = 3,
                Nome = "Vegetariano",
                Icone = "fas fa-leaf",
            },
            new()
            {
                Id = 4,
                Nome = "Carnes",
                Icone = "fas fa-drumstick-bite",
            },
            new()
            {
                Id = 5,
                Nome = "Doces",
                Icone = "fas fa-cake-candles",
            },
            new()
            {
                Id = 6,
                Nome = "Pães",
                Icone = "fas fa-bread-slice",
            },
            new()
            {
                Id = 7,
                Nome = "Sopas",
                Icone = "fas fa-mug-hot",
            },
            new()
            {
                Id = 8,
                Nome = "Picantes",
                Icone = "fas fa-pepper-hot",
            }
        };
        builder.Entity<Categoria>().HasData(categorias);
        #endregion
    
        #region Popular Ingredientes
        List<Ingrediente> ingredientes = new() {
            new Ingrediente() {
                Id = 1,
                Nome = "Carne Moída"
            },
            new Ingrediente() {
                Id = 2,
                Nome = "Pimentão Verde"
            },
            new Ingrediente() {
                Id = 3,
                Nome = "Pimentão Vermelho"
            },
            new Ingrediente() {
                Id = 4,
                Nome = "Pimentão Amarelo"
            },
            new Ingrediente() {
                Id = 5,
                Nome = "Cebola"
            },
            new Ingrediente() {
                Id = 6,
                Nome = "Curry"
            },
            new Ingrediente() {
                Id = 7,
                Nome = "Pimenta Calabresa"
            },
            new Ingrediente() {
                Id = 8,
                Nome = "Páprica Picante"
            },
            new Ingrediente() {
                Id = 9,
                Nome = "Sal"
            },
            new Ingrediente() {
                Id = 10,
                Nome = "Orégano"
            },
            new Ingrediente() {
                Id = 11,
                Nome = "Pão Sirio"
            },
            new Ingrediente() {
                Id = 12,
                Nome = "Cream Cheese"
            },
            new Ingrediente() {
                Id = 13,
                Nome = "Cheddar"
            },
            new Ingrediente() {
                Id = 14,
                Nome = "Azeite"
            }
        };
        builder.Entity<Ingrediente>().HasData(ingredientes);
        #endregion
    
        #region Populate Receita
        List<Receita> receitas = new() {
            new Receita() {
                Id = 1,
                Nome = "Carne Moída Mexicana",
                Descricao = "Prato perfeito para um lanche rápido ou mesmo uma refeição picante. Carne moída, pimentões, temperos e muito queijooooo",
                CategoriaId = 4,
                Dificuldade = Dificuldade.Fácil,
                Rendimento = 5,
                TempoPreparo = "20 minutos",
                Foto = "/img/receitas/1.jpg",
                UsuarioId = usuarios[0].Id,
                Destaque = true,
                DicaDoChef = "Você pode modificar a quantidade de temperos a gosto. Não esqueça de misturar bem os queijos."
            }
        };
        builder.Entity<Receita>().HasData(receitas);
        #endregion

        #region Popular Preparo
        List<Preparo> preparos = new()
        {
            new()
            {
                Id = 1,
                ReceitaId = 1,
                Texto = "Comece pela preparação dos ingredientes, pique os pimentões e a cebola em pequenos cubos, se preferir você também pode usar um processador de alimentos."
            },
            new()
            {
                Id = 2,
                ReceitaId = 1,
                Texto = "Coloque a carne moída para fritar em uma panela com um pouco de azeite."
            },
            new()
            {
                Id = 3,
                ReceitaId = 1,
                Texto = "Quando a carne moída já não estiver mais crua, adicione os pimentões e a cebola, mexendo bem para misturar todos os ingredientes."
            },
            new()
            {
                Id = 4,
                ReceitaId = 1,
                Texto = "Aguarde alguns instante e adicione os temperos, mexendo novamente para misturar."
            },
            new()
            {
                Id = 5,
                ReceitaId = 1,
                Texto = "Frite por mais alguns minutos a carne com os demais ingredientes."
            },
            new()
            {
                Id = 6,
                ReceitaId = 1,
                Texto = "Adicione o Cream Cheese e o Queijo Cheddar, mexendo bem para evitar que queime o fundo e ajudar os queijos a derreterem."
            },
            new()
            {
                Id = 7,
                ReceitaId = 1,
                Texto = "Quando os queijos já estiverem bem derretidos e misturados com os demais ingredientes, sirva acompanhado do Pão Sirio ou de Doritos."
            },
        };
        builder.Entity<Preparo>().HasData(preparos);
        #endregion

        #region Receita Ingrediente
        List<ReceitaIngrediente> receitaIngredientes = new() {
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 1,
                Quantidade = "500g"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 3,
                Quantidade = "1 pequeno"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 4,
                Quantidade = "1 pequeno"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 5,
                Quantidade = "1 pequeno"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 6,
                Quantidade = "1 colher sopa"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 7,
                Quantidade = "1 colher sopa"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 8,
                Quantidade = "1 colher sopa"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 9,
                Quantidade = "1 colher sopa"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 10,
                Quantidade = "1 colher sopa"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 11,
                Quantidade = "A vontade"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 12,
                Quantidade = "200g"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 13,
                Quantidade = "200g"
            },
            new ReceitaIngrediente() {
                ReceitaId = 1,
                IngredienteId = 14,
                Quantidade = "Um pouco"
            }
        };
        builder.Entity<ReceitaIngrediente>().HasData(receitaIngredientes);
        #endregion
    }
}
