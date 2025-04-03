using DietDoHongTran.Models;
using Microsoft.AspNetCore.Mvc;

namespace DietDoHongTran.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private Cart GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cart = session.GetObjectFromJson<Cart>("Cart") ?? new Cart();
            return cart;
        }

        private void SaveCart(Cart cart)
        {
            _httpContextAccessor.HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            int itemCount = cart.Items.Sum(i => i.Quantity);
            return Json(new { count = itemCount });
        }


        public IActionResult AddToCart(int productId, string productName, decimal price, int quantity = 1)
        {
            var cart = GetCart();
            cart.AddItem(new CartItem { ProductId = productId, ProductName = productName, Price = price, Quantity = quantity });
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveItem(productId);
            SaveCart(cart);
            return RedirectToAction("Index");
        }
    }
}
