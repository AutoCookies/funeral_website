using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietDoHongTran.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DietDoHongTran.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoiceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> EnterShippingAddress()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var shoppingCart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(sc => sc.ApplicationUserId == user.Id);

            if (shoppingCart == null || !shoppingCart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "ShoppingCart");
            }

            var model = new Invoice
            {
                TotalAmount = shoppingCart.Items.Sum(i => i.Product.Price * i.Quantity),
                ShippingAddress = "",
                ShippingCity = "",
                ShippingPostalCode = "",
                ShippingCountry = ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoiceFromCart(Invoice model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var shoppingCart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(sc => sc.ApplicationUserId == user.Id);

            if (shoppingCart == null || !shoppingCart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "ShoppingCart");
            }

            // ✅ Gán ApplicationUserId và khởi tạo InvoiceDetails
            var invoice = new Invoice
            {
                ApplicationUserId = user.Id,  // 🛠 FIX lỗi thiếu ApplicationUserId
                TotalAmount = shoppingCart.Items.Sum(i => i.Product.Price * i.Quantity),
                CreatedAt = DateTime.Now,
                IsPaid = true,
                ShippingAddress = model.ShippingAddress,
                ShippingCity = model.ShippingCity,
                ShippingPostalCode = model.ShippingPostalCode,
                ShippingCountry = model.ShippingCountry,
                InvoiceDetails = new List<InvoiceDetail>()  // 🛠 FIX lỗi null
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(); // Lưu để có InvoiceId

            // ✅ Thêm chi tiết hóa đơn
            foreach (var item in shoppingCart.Items)
            {
                var invoiceDetail = new InvoiceDetail
                {
                    InvoiceId = invoice.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };

                invoice.InvoiceDetails.Add(invoiceDetail);
                _context.InvoiceDetails.Add(invoiceDetail);
            }

            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi thanh toán thành công
            _context.ShoppingCarts.Remove(shoppingCart);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Hóa đơn đã được tạo thành công!");

            return RedirectToAction("InvoiceDetails", new { id = invoice.Id });
        }

        public async Task<IActionResult> InvoiceDetails(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Tạo dictionary ánh xạ ProductId -> ProductName
            var productNames = await _context.Products
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            ViewBag.ProductNames = productNames;

            return View(invoice);
        }

        // Action để hiển thị trang cảm ơn
        public IActionResult ThankYou()
        {
            return View(); // Bạn có thể tạo trang cảm ơn ở đây
        }
    }
}
