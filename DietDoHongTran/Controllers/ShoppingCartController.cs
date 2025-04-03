using DietDoHongTran.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DietDoHongTran.Controllers
{
    [Authorize] // Yêu cầu đăng nhập để truy cập ShoppingCart
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 📌 Lấy giỏ hàng của người dùng
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var shoppingCart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(sc => sc.ApplicationUserId == userId);

            return View(shoppingCart);
        }

        // 📌 Thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var shoppingCart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefaultAsync(sc => sc.ApplicationUserId == userId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    Items = new List<CartItem>()
                };
                _context.ShoppingCarts.Add(shoppingCart);
            }

            var cartItem = shoppingCart.Items.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                };
                shoppingCart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 📌 Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null) return NotFound();

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 📌 Xóa một sản phẩm khỏi giỏ hàng
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null) return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 📌 Xóa toàn bộ giỏ hàng
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var shoppingCart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                .FirstOrDefaultAsync(sc => sc.ApplicationUserId == userId);

            if (shoppingCart != null)
            {
                _context.CartItems.RemoveRange(shoppingCart.Items);
                shoppingCart.Items.Clear();
                _context.ShoppingCarts.Update(shoppingCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var cart = _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.ApplicationUserId == userId);

            int itemCount = cart?.Items.Sum(i => i.Quantity) ?? 0;

            return Json(new { count = itemCount });
        }

    }
}
