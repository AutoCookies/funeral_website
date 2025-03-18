using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DietDoHongTran.ViewModels;

namespace DietDoHongTran.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceProduct> ServiceProducts { get; set; } // Thêm DbSet bảng trung gian

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Định nghĩa quan hệ nhiều-nhiều thông qua bảng trung gian ServiceProduct
            modelBuilder.Entity<ServiceProduct>()
                .HasKey(sp => new { sp.ServiceId, sp.ProductId }); // Định nghĩa khóa chính

            modelBuilder.Entity<ServiceProduct>()
                .HasOne(sp => sp.Service)
                .WithMany(s => s.ServiceProducts)
                .HasForeignKey(sp => sp.ServiceId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Service thì ServiceProduct cũng bị xóa

            modelBuilder.Entity<ServiceProduct>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.ServiceProducts)
                .HasForeignKey(sp => sp.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Product thì ServiceProduct cũng bị xóa

            // Tắt Cascade Delete giữa Category và Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)"); // Chỉ định kiểu decimal với precision là 18 và scale là 2

            modelBuilder.Entity<Service>()
                .Property(s => s.BasePrice)
                .HasColumnType("decimal(18,2)");

            // Định nghĩa khóa chính và auto-increment cho các bảng
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().Property(c => c.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Service>().HasKey(s => s.Id);
            modelBuilder.Entity<Service>().Property(s => s.Id).ValueGeneratedOnAdd();
        }
    }
}
