using SegredoDaCozinha.Data;
using SegredoDaCozinha.Models;
using SegredoDaCozinha.Services;          // ← ADICIONADO: necessário para o IUsuarioService
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Serviço de conexão do Contexto
string conexao = builder.Configuration.GetConnectionString("Conexao");
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseMySQL(conexao)
);

// Configuração do serviço do Identity
builder.Services.AddIdentity<Usuario, IdentityRole>(
    options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
    }
)
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Registrar o serviço de usuário (para login e informações do usuário)
builder.Services.AddTransient<IUsuarioService, UsuarioService>();  // ← ADICIONADO

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Código para garantir a existência do banco ao executar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();  // Verifica se o usuário está logado
app.UseAuthorization();   // Verifica se o usuário tem permissão

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();