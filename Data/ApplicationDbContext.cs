using Ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductSpecDetail> ProductSpecDetails { get; set; }
        public DbSet<ProductSpecGroup> ProductSpecGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Product - Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình OrderProduct
            modelBuilder.Entity<OrderProduct>(entity =>
            {
                // Khóa chính tự tăng
                entity.HasKey(op => op.Id);
                entity.Property(op => op.Id)
                    .UseIdentityColumn();

                // Quan hệ với Order
                entity.HasOne(op => op.Order)
                    .WithMany(o => o.OrderProducts)
                    .HasForeignKey(op => op.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ với Product
                entity.HasOne(op => op.Product)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(op => op.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ràng buộc duy nhất cho cặp OrderId-ProductId
                entity.HasIndex(op => new { op.OrderId, op.ProductId })
                    .IsUnique();
            });

            // Cấu hình Product Specification
            modelBuilder.Entity<ProductSpecDetail>()
                .HasOne(p => p.SpecGroup)
                .WithMany(g => g.Specifications)
                .HasForeignKey(p => p.SpecGroupId);
        }
    }
}