using DietDoHongTran.Models;

namespace DietDoHongTran.ViewModels
{
    public class ServiceProduct
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
