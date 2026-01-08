using Microsoft.EntityFrameworkCore;
using TurismoApp.Models;

namespace TurismoApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PontoTuristico> PontosTuristicos { get; set; }
    }
}

