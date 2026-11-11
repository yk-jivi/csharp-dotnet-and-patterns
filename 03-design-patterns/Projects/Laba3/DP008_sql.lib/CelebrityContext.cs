using Microsoft.EntityFrameworkCore;

namespace DP008_sql.lib
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