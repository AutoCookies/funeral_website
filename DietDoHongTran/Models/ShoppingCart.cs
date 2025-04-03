using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DietDoHongTran.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        // Liên kết với ApplicationUser
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        // ShoppingCart có nhiều CartItem
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
