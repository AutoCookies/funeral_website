using System.ComponentModel.DataAnnotations;

namespace DietDoHongTran.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "ApplicationUserId is required")]
        public string ApplicationUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }

        // Thêm các trường địa chỉ giao hàng
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
