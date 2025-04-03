using DietDoHongTran.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DietDoHongTran.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, Range(0.01, 10000000.000)]
        public decimal Price { get; set; }

        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int Instock { get; set; }
        public int Sold { get; set; } = 0;

        // Danh sách liên kết qua bảng trung gian
        public List<ServiceProduct>? ServiceProducts { get; set; }
    }
}
