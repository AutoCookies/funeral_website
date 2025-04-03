using DietDoHongTran.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DietDoHongTran.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? BasePrice { get; set; }

        // Danh sách liên kết qua bảng trung gian
        public List<ServiceProduct>? ServiceProducts { get; set; }

        public decimal ComputedTotalPrice => (BasePrice ?? 0) + (ServiceProducts?.Sum(sp => sp.Product.Price) ?? 0);
    }

}
