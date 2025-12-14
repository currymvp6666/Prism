using Microsoft.EntityFrameworkCore;

using Prism.Model;

namespace Prism.Data
{
    public class PrismDbContext : DbContext
    {
        public DbSet<Memo> Memos { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=郭;Database=PrismDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
