using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DietDoHongTran.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tắt Cascade Delete giữa Category và Product (set CategoryId to null for products when deleting Category)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .OnDelete(DeleteBehavior.SetNull); // Ensures products don't get deleted but CategoryId is set to null

            // Explicitly setting the primary key for Category (EF Core does this by default)
            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id);

            // Ensuring CategoryId is auto-generated (auto-increment)
            modelBuilder.Entity<Category>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            // Explicitly setting the primary key for Product (EF Core does this by default)
            modelBuilder.Entity<Product>()
                .HasKey(p => p.Id);

            // Ensuring ProductId is auto-generated (auto-increment)
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
