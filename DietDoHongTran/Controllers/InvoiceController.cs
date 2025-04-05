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

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra số lượng mua so với InStock trước khi tạo hóa đơn
                    foreach (var item in shoppingCart.Items)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product == null)
                        {
                            ModelState.AddModelError("", $"Sản phẩm với ID {item.ProductId} không tồn tại.");
                            return View(model);
                        }

                        if (product.Instock == 0)
                        {
                            ModelState.AddModelError("", $"Sản phẩm {product.Name} đã hết hàng.");
                            return View(model);
                        }

                        if (product.Instock < item.Quantity)
                        {
                            ModelState.AddModelError("", $"Không đủ số lượng tồn kho cho sản phẩm {product.Name}.");
                            return View(model);
                        }
                    }

                    // Tạo hóa đơn
                    var invoice = new Invoice
                    {
                        ApplicationUserId = user.Id,
                        TotalAmount = shoppingCart.Items.Sum(i => i.Product.Price * i.Quantity),
                        CreatedAt = DateTime.Now,
                        IsPaid = true,
                        ShippingAddress = model.ShippingAddress,
                        ShippingCity = model.ShippingCity,
                        ShippingPostalCode = model.ShippingPostalCode,
                        ShippingCountry = model.ShippingCountry,
                        InvoiceDetails = new List<InvoiceDetail>()
                    };

                    _context.Invoices.Add(invoice);
                    await _context.SaveChangesAsync();

                    // Tạo chi tiết hóa đơn và cập nhật InStock
                    foreach (var item in shoppingCart.Items)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);

                        var invoiceDetail = new InvoiceDetail
                        {
                            InvoiceId = invoice.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Product.Price
                        };

                        invoice.InvoiceDetails.Add(invoiceDetail);
                        _context.InvoiceDetails.Add(invoiceDetail);

                        product.Instock -= item.Quantity;
                        _context.Products.Update(product);
                    }

                    await _context.SaveChangesAsync();

                    // Xóa giỏ hàng sau khi thanh toán thành công
                    _context.ShoppingCarts.Remove(shoppingCart);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return RedirectToAction("InvoiceDetails", new { id = invoice.Id });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình thanh toán.");
                    // Ghi log lỗi (tùy chọn)
                    Console.WriteLine($"Lỗi thanh toán: {ex.Message}");
                    return View(model);
                }
            }
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

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Lấy danh sách hóa đơn của user
            var invoices = await _context.Invoices
                .Where(i => i.ApplicationUserId == userId)
                .Include(i => i.InvoiceDetails)  // Bao gồm chi tiết hóa đơn
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            if (!invoices.Any())
            {
                TempData["ErrorMessage"] = "Bạn chưa có hóa đơn nào!";
            }

            // Lấy danh sách ProductId từ InvoiceDetails
            var productIds = invoices.SelectMany(i => i.InvoiceDetails)
                .Select(d => d.ProductId)
                .Distinct()
                .ToList();

            // Lấy thông tin sản phẩm từ ProductId
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Tạo dictionary ánh xạ ProductId -> ProductName
            var productNames = products.ToDictionary(p => p.Id, p => p.Name);

            // Gán sản phẩm vào ViewBag để truy cập trong View
            ViewBag.ProductNames = productNames;

            return View(invoices);  // Trả về danh sách hóa đơn
        }
    }
}
