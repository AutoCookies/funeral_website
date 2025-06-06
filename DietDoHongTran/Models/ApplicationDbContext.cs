﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        public DbSet<CartItem> CartItems { get; set; } // Thêm DbSet cho CartItem
        public DbSet<ProductStatistic> ProductStatistics { get; set; } // Thêm DbSet cho ProductStatistic

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Thiết lập mối quan hệ giữa ApplicationUser và ShoppingCart
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.ShoppingCart)
                .WithOne(sc => sc.ApplicationUser)
                .HasForeignKey<ShoppingCart>(sc => sc.ApplicationUserId);

            // Thiết lập mối quan hệ giữa ShoppingCart và CartItem
            modelBuilder.Entity<ShoppingCart>()
                .HasMany(sc => sc.Items)
                .WithOne(ci => ci.ShoppingCart)
                .HasForeignKey(ci => ci.ShoppingCartId);

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
