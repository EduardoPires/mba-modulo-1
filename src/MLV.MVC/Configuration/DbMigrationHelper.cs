using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MLV.Business.Data;
using MLV.Business.Models;

namespace MLV.MVC.Configuration;

public static class DbMigrationHelper
{
    public static async Task EnsureSeedData(WebApplication serviceScope)
    {
        var services = serviceScope.Services.CreateScope().ServiceProvider;
        await EnsureSeedData(services);
    }

    public static async Task EnsureSeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        var context = scope.ServiceProvider.GetRequiredService<MlvDbContext>();

        if (env.IsDevelopment())
        {
            await context.Database.MigrateAsync();
            await EnsureSeedCategorias(context);
        }
    }

    public static async Task EnsureSeedCategorias(MlvDbContext context)
    {
        var userId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();
        var nome = "admin";
        var email = "admin@admin.com.br";

        if (!context.Users.Any())
        {
            await context.Users.AddAsync(new IdentityUser
            {
                Id = userId.ToString(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                AccessFailedCount = 0,
                LockoutEnabled = false,
                PasswordHash = "AQAAAAIAAYagAAAAEGAzzcKRwIUJd07YaEH6wCPNnXQbRCblHTVbTI0Le/LzSPYvakHu/V50sl6brtNmZw==",
                TwoFactorEnabled = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            });

            await context.Vendedores.AddAsync(new Vendedor(userId, nome, email, nome, email));

            await context.SaveChangesAsync();
        }

        if (!context.Categorias.Any())
        {
            await context.Categorias.AddAsync(new Categoria(categoriaId, "Eletrônicos, TV e Áudio", null, email));
            await context.SaveChangesAsync();
        }

        if (!context.Produtos.Any())
        {
            await context.Produtos.AddAsync(new Produto(Guid.NewGuid(), categoriaId, userId, "Headphone Philips bluetooth on-ear com microfone e energia para 15 horas na cor preto TAH1108BK/55", "Tenha a experiência sonora Philips. Curta cada minuto de suas músicas e conteúdos com um som nítido e com todas as frequências aguda, média e grave bem definidas", 102, 100, "/uploads/imagem-base.jpg", email));
            await context.SaveChangesAsync();
        }
    }
}
