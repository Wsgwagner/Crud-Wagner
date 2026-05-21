using Microsoft.EntityFrameworkCore;

namespace WebCRUDMVCSQL.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {

        }

        public DbSet<Produto> Produto { get; set; } = default!;

        public DbSet<Clientes> Clientes { get; set; } = default!;   

        public DbSet<Usuarios> Usuarios { get; set; } = default!;

    }
}
