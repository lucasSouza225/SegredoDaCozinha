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
                NormalizedUserName = "ADMINISTRADOR@SEGREDODACOZINHA.COM",
                Email = "administrador@SegredoDaCozinha.com",
                NormalizedEmail = "ADMINISTRADOR@SEGREDODACOZINHA.COM",
                DataNascimento = DateTime.Parse("05/08/1981"),
                Foto = "/img/usuarios/perfil.jpg",
                EmailConfirmed = true,
                LockoutEnabled = false
            },
            new()
            {
                Id = "0cbf5d2b-b2d5-4b06-bb4d-3b353b10e2b0",
                Nome = "Lucas de Souza Santos",
                UserName = "lucas@gmail.com",
                NormalizedUserName = "LUCAS@GMAIL.COM",
                Email = "lucas@gmail.com",
                NormalizedEmail = "LUCAS@GMAIL.COM",
                DataNascimento = DateTime.Parse("15/03/1995"),
                Foto = "/img/usuarios/perfil.jpg",
                EmailConfirmed = true,
                LockoutEnabled = true
            }
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
            new() { UserId = usuarios[0].Id, RoleId = roles[0].Id },  // Administrador
            new() { UserId = usuarios[1].Id, RoleId = roles[2].Id }   // Usuário comum
        };
        builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        #endregion
    
        #region Popular Categoria
        List<Categoria> categorias = new()
        {
            new() { Id = 1, Nome = "Massas", Icone = "fas fa-pizza-slice" },
            new() { Id = 2, Nome = "Peixes", Icone = "fas fa-fish" },
            new() { Id = 3, Nome = "Vegetariano", Icone = "fas fa-leaf" },
            new() { Id = 4, Nome = "Carnes", Icone = "fas fa-drumstick-bite" },
            new() { Id = 5, Nome = "Doces", Icone = "fas fa-cake-candles" },
            new() { Id = 6, Nome = "Pães", Icone = "fas fa-bread-slice" },
            new() { Id = 7, Nome = "Sopas", Icone = "fas fa-mug-hot" },
            new() { Id = 8, Nome = "Picantes", Icone = "fas fa-pepper-hot" },
            new() { Id = 9, Nome = "Café da Manhã", Icone = "fas fa-coffee" },
            new() { Id = 10, Nome = "Lanches", Icone = "fas fa-hamburger" }
        };
        builder.Entity<Categoria>().HasData(categorias);
        #endregion
    
        #region Popular Ingredientes
        List<Ingrediente> ingredientes = new() 
        {
            new() { Id = 1, Nome = "Carne Moída" },
            new() { Id = 2, Nome = "Pimentão Verde" },
            new() { Id = 3, Nome = "Pimentão Vermelho" },
            new() { Id = 4, Nome = "Pimentão Amarelo" },
            new() { Id = 5, Nome = "Cebola" },
            new() { Id = 6, Nome = "Curry" },
            new() { Id = 7, Nome = "Pimenta Calabresa" },
            new() { Id = 8, Nome = "Páprica Picante" },
            new() { Id = 9, Nome = "Sal" },
            new() { Id = 10, Nome = "Orégano" },
            new() { Id = 11, Nome = "Pão Sirio" },
            new() { Id = 12, Nome = "Cream Cheese" },
            new() { Id = 13, Nome = "Cheddar" },
            new() { Id = 14, Nome = "Azeite" },
            new() { Id = 15, Nome = "Farinha de Trigo" },
            new() { Id = 16, Nome = "Açúcar" },
            new() { Id = 17, Nome = "Ovos" },
            new() { Id = 18, Nome = "Leite" },
            new() { Id = 19, Nome = "Manteiga" },
            new() { Id = 20, Nome = "Fermento" },
            new() { Id = 21, Nome = "Chocolate em Pó" },
            new() { Id = 22, Nome = "Cenoura" },
            new() { Id = 23, Nome = "Laranja" },
            new() { Id = 24, Nome = "Tomate" },
            new() { Id = 25, Nome = "Alho" },
            new() { Id = 26, Nome = "Arroz" },
            new() { Id = 27, Nome = "Feijão" },
            new() { Id = 28, Nome = "Frango" },
            new() { Id = 29, Nome = "Brócolis" },
            new() { Id = 30, Nome = "Queijo Mussarela" }
        };
        builder.Entity<Ingrediente>().HasData(ingredientes);
        #endregion
    
        #region Popular Receitas (COM URLs DE IMAGENS ONLINE)
        List<Receita> receitas = new() 
        {
            new() 
            {
                Id = 1,
                Nome = "Carne Moída Mexicana",
                Descricao = "Prato perfeito para um lanche rápido ou mesmo uma refeição picante. Carne moída, pimentões, temperos e muito queijo!",
                CategoriaId = 4,
                Dificuldade = Dificuldade.Fácil,
                Rendimento = 5,
                TempoPreparo = "20 minutos",
                Foto = "https://images.unsplash.com/photo-1600891964092-4316c288032e?w=800&h=600&fit=crop",
                UsuarioId = usuarios[0].Id,
                Destaque = true,
                DicaDoChef = "Você pode modificar a quantidade de temperos a gosto. Não esqueça de misturar bem os queijos."
            },
            new() 
            {
                Id = 2,
                Nome = "Bolo de Cenoura com Cobertura",
                Descricao = "Bolo fofinho, úmido e com a clássica cobertura de chocolate. Perfeito para o café da tarde!",
                CategoriaId = 5,
                Dificuldade = Dificuldade.Fácil,
                Rendimento = 8,
                TempoPreparo = "45 minutos",
                Foto = "https://images.unsplash.com/photo-1578774296842-c45e472b5c2d?w=800&h=600&fit=crop",
                UsuarioId = usuarios[0].Id,
                Destaque = true,
                DicaDoChef = "Use cenouras bem laranjas e bata bastante a massa para ficar bem fofinha."
            },
            new() 
            {
                Id = 3,
                Nome = "Frango ao Curry com Arroz",
                Descricao = "Frango suculento com molho cremoso de curry, acompanhado de arroz branco. Uma explosão de sabores!",
                CategoriaId = 4,
                Dificuldade = Dificuldade.Médio,
                Rendimento = 4,
                TempoPreparo = "35 minutos",
                Foto = "https://images.unsplash.com/photo-1603894584373-5ac82b2ae398?w=800&h=600&fit=crop",
                UsuarioId = usuarios[1].Id,
                Destaque = false,
                DicaDoChef = "Deixe o frango marinar no curry por 30 minutos antes de cozinhar para intensificar o sabor."
            },
            new() 
            {
                Id = 4,
                Nome = "Salada Especial de Brócolis",
                Descricao = "Salada refrescante com brócolis, tomates e um molho especial. Saudável e deliciosa!",
                CategoriaId = 3,
                Dificuldade = Dificuldade.Fácil,
                Rendimento = 4,
                TempoPreparo = "15 minutos",
                Foto = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800&h=600&fit=crop",
                UsuarioId = usuarios[1].Id,
                Destaque = false,
                DicaDoChef = "Branqueie os brócolis por 2 minutos para ficarem mais crocantes e verdes."
            },
            new() 
            {
                Id = 5,
                Nome = "Feijoada Light",
                Descricao = "Feijoada mais leve, com carnes magras e muito sabor. Perfeita para um almoço de fim de semana!",
                CategoriaId = 7,
                Dificuldade = Dificuldade.Médio,
                Rendimento = 6,
                TempoPreparo = "1h 30min",
                Foto = "https://images.unsplash.com/photo-1601493714580-2c3e5e3d3b5a?w=800&h=600&fit=crop",
                UsuarioId = usuarios[0].Id,
                Destaque = false,
                DicaDoChef = "Deixe o feijão de molho de um dia para o outro para cozinhar mais rápido."
            }
        };
        builder.Entity<Receita>().HasData(receitas);
        #endregion

        #region Popular Preparo
        List<Preparo> preparos = new()
        {
            new() { Id = 1, ReceitaId = 1, Texto = "Comece picando os pimentões e a cebola em pequenos cubos." },
            new() { Id = 2, ReceitaId = 1, Texto = "Frite a carne moída em uma panela com azeite." },
            new() { Id = 3, ReceitaId = 1, Texto = "Adicione os pimentões e a cebola, mexendo bem." },
            new() { Id = 4, ReceitaId = 1, Texto = "Adicione os temperos (curry, pimenta, páprica, sal, orégano)." },
            new() { Id = 5, ReceitaId = 1, Texto = "Frite por mais alguns minutos." },
            new() { Id = 6, ReceitaId = 1, Texto = "Adicione Cream Cheese e Cheddar, mexendo até derreter." },
            new() { Id = 7, ReceitaId = 1, Texto = "Sirva com pão sírio ou doritos." },
            new() { Id = 8, ReceitaId = 2, Texto = "Preaqueça o forno a 180°C. Unte uma forma com manteiga e farinha." },
            new() { Id = 9, ReceitaId = 2, Texto = "Bata no liquidificador as cenouras, ovos, óleo e açúcar." },
            new() { Id = 10, ReceitaId = 2, Texto = "Em uma tigela, misture a farinha e o fermento." },
            new() { Id = 11, ReceitaId = 2, Texto = "Adicione a mistura do liquidificador e mexa até homogeneizar." },
            new() { Id = 12, ReceitaId = 2, Texto = "Asse por cerca de 40 minutos." },
            new() { Id = 13, ReceitaId = 2, Texto = "Para a cobertura, misture chocolate, manteiga e leite." },
            new() { Id = 14, ReceitaId = 2, Texto = "Despeje sobre o bolo ainda quente." },
            new() { Id = 15, ReceitaId = 3, Texto = "Corte o frango em cubos e tempere com sal e curry." },
            new() { Id = 16, ReceitaId = 3, Texto = "Refogue a cebola e o alho no azeite." },
            new() { Id = 17, ReceitaId = 3, Texto = "Adicione o frango e doure bem." },
            new() { Id = 18, ReceitaId = 3, Texto = "Acrescente o creme de leite e mais curry a gosto." },
            new() { Id = 19, ReceitaId = 3, Texto = "Cozinhe por 10 minutos em fogo baixo." },
            new() { Id = 20, ReceitaId = 3, Texto = "Sirva com arroz branco." },
            new() { Id = 21, ReceitaId = 4, Texto = "Lave bem os brócolis e corte em buquês." },
            new() { Id = 22, ReceitaId = 4, Texto = "Cozinhe os brócolis em água fervente por 2 minutos e escorra." },
            new() { Id = 23, ReceitaId = 4, Texto = "Pique os tomates e a cebola em cubos pequenos." },
            new() { Id = 24, ReceitaId = 4, Texto = "Misture todos os ingredientes em uma tigela." },
            new() { Id = 25, ReceitaId = 4, Texto = "Tempere com azeite, sal e limão." },
            new() { Id = 26, ReceitaId = 4, Texto = "Leve à geladeira por 30 minutos antes de servir." },
            new() { Id = 27, ReceitaId = 5, Texto = "Deixe o feijão de molho por 12 horas." },
            new() { Id = 28, ReceitaId = 5, Texto = "Cozinhe o feijão na panela de pressão por 40 minutos." },
            new() { Id = 29, ReceitaId = 5, Texto = "Em outra panela, refogue a cebola, alho e as carnes." },
            new() { Id = 30, ReceitaId = 5, Texto = "Adicione o feijão cozido e deixe apurar por 30 minutos." },
            new() { Id = 31, ReceitaId = 5, Texto = "Tempere com sal e louro." },
            new() { Id = 32, ReceitaId = 5, Texto = "Sirva com arroz, couve e farofa." }
        };
        builder.Entity<Preparo>().HasData(preparos);
        #endregion

        #region Receita Ingrediente
        List<ReceitaIngrediente> receitaIngredientes = new() 
        {
            new() { ReceitaId = 1, IngredienteId = 1, Quantidade = "500g" },
            new() { ReceitaId = 1, IngredienteId = 3, Quantidade = "1 pequeno" },
            new() { ReceitaId = 1, IngredienteId = 4, Quantidade = "1 pequeno" },
            new() { ReceitaId = 1, IngredienteId = 5, Quantidade = "1 pequeno" },
            new() { ReceitaId = 1, IngredienteId = 6, Quantidade = "1 colher" },
            new() { ReceitaId = 1, IngredienteId = 7, Quantidade = "1 colher" },
            new() { ReceitaId = 1, IngredienteId = 8, Quantidade = "1 colher" },
            new() { ReceitaId = 1, IngredienteId = 9, Quantidade = "a gosto" },
            new() { ReceitaId = 1, IngredienteId = 10, Quantidade = "1 colher" },
            new() { ReceitaId = 1, IngredienteId = 11, Quantidade = "para servir" },
            new() { ReceitaId = 1, IngredienteId = 12, Quantidade = "200g" },
            new() { ReceitaId = 1, IngredienteId = 13, Quantidade = "200g" },
            new() { ReceitaId = 1, IngredienteId = 14, Quantidade = "2 colheres" },
            new() { ReceitaId = 2, IngredienteId = 22, Quantidade = "3 unidades" },
            new() { ReceitaId = 2, IngredienteId = 17, Quantidade = "4 unidades" },
            new() { ReceitaId = 2, IngredienteId = 16, Quantidade = "2 xícaras" },
            new() { ReceitaId = 2, IngredienteId = 14, Quantidade = "1 xícara" },
            new() { ReceitaId = 2, IngredienteId = 15, Quantidade = "2 xícaras" },
            new() { ReceitaId = 2, IngredienteId = 20, Quantidade = "1 colher" },
            new() { ReceitaId = 2, IngredienteId = 21, Quantidade = "3 colheres" },
            new() { ReceitaId = 2, IngredienteId = 19, Quantidade = "1 colher" },
            new() { ReceitaId = 2, IngredienteId = 18, Quantidade = "3 colheres" },
            new() { ReceitaId = 3, IngredienteId = 28, Quantidade = "500g" },
            new() { ReceitaId = 3, IngredienteId = 5, Quantidade = "1 unidade" },
            new() { ReceitaId = 3, IngredienteId = 25, Quantidade = "3 dentes" },
            new() { ReceitaId = 3, IngredienteId = 6, Quantidade = "2 colheres" },
            new() { ReceitaId = 3, IngredienteId = 9, Quantidade = "a gosto" },
            new() { ReceitaId = 3, IngredienteId = 14, Quantidade = "2 colheres" },
            new() { ReceitaId = 3, IngredienteId = 18, Quantidade = "1 lata" },
            new() { ReceitaId = 4, IngredienteId = 29, Quantidade = "2 maços" },
            new() { ReceitaId = 4, IngredienteId = 24, Quantidade = "2 unidades" },
            new() { ReceitaId = 4, IngredienteId = 5, Quantidade = "1 unidade" },
            new() { ReceitaId = 4, IngredienteId = 14, Quantidade = "3 colheres" },
            new() { ReceitaId = 4, IngredienteId = 9, Quantidade = "a gosto" },
            new() { ReceitaId = 4, IngredienteId = 23, Quantidade = "1 unidade" },
            new() { ReceitaId = 5, IngredienteId = 27, Quantidade = "500g" },
            new() { ReceitaId = 5, IngredienteId = 5, Quantidade = "2 unidades" },
            new() { ReceitaId = 5, IngredienteId = 25, Quantidade = "4 dentes" },
            new() { ReceitaId = 5, IngredienteId = 9, Quantidade = "a gosto" },
            new() { ReceitaId = 5, IngredienteId = 28, Quantidade = "300g" }
        };
        builder.Entity<ReceitaIngrediente>().HasData(receitaIngredientes);
        #endregion
    }
}