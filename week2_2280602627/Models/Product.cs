using System.ComponentModel.DataAnnotations;
namespace week2_2280602627
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; } // Link ảnh từ người dùng cung cấp
        public List<string>? ImageUrls { get; set; } // Danh sách ảnh tải lên
    }
}

