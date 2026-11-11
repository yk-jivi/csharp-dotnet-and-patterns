using Microsoft.EntityFrameworkCore;
using ServiceLocator.Interfaces;

namespace ServiceLocator.Library
{
    public class CelebrityContext : DbContext
    {
        public DbSet<Celebrity> Celebrities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=CelebrityDB;Integrated Security=True;TrustServerCertificate=True;");
        }
    }
}