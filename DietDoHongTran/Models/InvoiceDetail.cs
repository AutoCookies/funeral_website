namespace DietDoHongTran.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; } // Liên kết với Invoice
        public int ProductId { get; set; } // Liên kết với Product
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Invoice Invoice { get; set; }
    }
}
