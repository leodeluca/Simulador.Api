using Microsoft.EntityFrameworkCore;
using Simulador.Api.Models;

namespace Simulador.Api.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Simulacao> Simulacoes => Set<Simulacao>();
        public DbSet<Investimento> Investimentos => Set<Investimento>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Perfil> Perfis => Set<Perfil>();
        public DbSet<Cliente> Clientes => Set<Cliente>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
