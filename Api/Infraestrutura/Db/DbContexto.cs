using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;


namespace Api;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options) { }

    public DbSet<Administrador> Administradores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var senhaHash = HashPassword("123456", workFactor: 11);

        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "admin@teste.com.br",
                Senha = senhaHash,
                Perfil = "Adm"
            }
        );
    }
}
