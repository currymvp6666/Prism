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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 设置 Category - Memo 关系：一对多
            modelBuilder.Entity<Memo>()
                        .HasOne(m => m.Category)
                        .WithMany(c => c.Memos)
                        .HasForeignKey(m => m.CategoryId)
                        .OnDelete(DeleteBehavior.Cascade);

            // 可选：给分类表默认数据
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "默认分类" }
            );
        }
    }
}
