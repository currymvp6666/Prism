using Microsoft.EntityFrameworkCore;

using Prism.Model;
using Prism.Models;

namespace Prism.Data
{
    public class PrismDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Memo> Memos { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Login> Logins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=郭;Database=PrismDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =========================
            // Category
            // =========================
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            // =========================
            // Memo - Category (一对多)
            // =========================
            modelBuilder.Entity<Memo>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Memos)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Memo>()
                .Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(100);

            // =========================
            // TodoItem - Category (一对多)
            // =========================
            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.TodoItems)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TodoItem>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);
            // =========================
            // Login 实体配置
            // =========================
            modelBuilder.Entity<Login>(entity =>
            {
                // 指定 UserID 为主键
                entity.HasKey(l => l.UserID);

                // 配置 UserName：必填，长度 50，且在数据库中唯一
                entity.Property(l => l.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(l => l.UserName)
                    .IsUnique();

                // 配置 UserPsw：必填，长度 100（为了后续存储加密后的哈希值，建议设长一点）
                entity.Property(l => l.UserPsw)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // =========================
            // 种子数据（关键，防 FK 报错）
            // =========================
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "默认" },
                new Category { Id = 2, Name = "工作" },
                new Category { Id = 3, Name = "学习" }
            );
        }
    }
}
