using System.ComponentModel.DataAnnotations;

namespace DietDoHongTran.Models
{
    public class ShippingAddressViewModel
    {
        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string ShippingCity { get; set; }

        [Required]
        public string ShippingPostalCode { get; set; }

        [Required]
        public string ShippingCountry { get; set; }
    }
}
