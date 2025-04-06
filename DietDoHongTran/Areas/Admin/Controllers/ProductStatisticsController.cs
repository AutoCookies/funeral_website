using DietDoHongTran.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DietDoHongTran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductStatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductStatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;

            // Tính toán thống kê
            var totalProducts = _context.Products.Count();
            var totalInStock = _context.Products.Sum(p => p.Instock);
            var totalSold = _context.Products.Sum(p => p.Sold);
            var totalValueInStock = _context.Products.Sum(p => p.Instock * p.Price);

            // Kiểm tra xem đã có thống kê cho hôm nay chưa
            var existingStat = _context.ProductStatistics.FirstOrDefault(ps => ps.Date.Date == today);
            if (existingStat == null)
            {
                var newStat = new ProductStatistic
                {
                    Date = today,
                    TotalProducts = totalProducts,
                    TotalInStock = totalInStock,
                    TotalSold = totalSold,
                    TotalValueInStock = totalValueInStock
                };
                _context.ProductStatistics.Add(newStat);
            }
            else
            {
                existingStat.TotalProducts = totalProducts;
                existingStat.TotalInStock = totalInStock;
                existingStat.TotalSold = totalSold;
                existingStat.TotalValueInStock = totalValueInStock;
                _context.ProductStatistics.Update(existingStat);
            }
            _context.SaveChanges();

            // Các thống kê khác
            var topSelling = _context.Products
                .OrderByDescending(p => p.Sold)
                .Take(5)
                .ToList();

            var outOfStock = _context.Products
                .Where(p => p.Instock == 0)
                .ToList();

            var lowStock = _context.Products
                .Where(p => p.Instock <= 5)
                .ToList();

            var productsByCategory = _context.Products
                .GroupBy(p => p.Category.Name)
                .Select(group => new CategoryStatistics
                {
                    Date = today,
                    Category = group.Key,
                    TotalProducts = group.Count(),
                    TotalSold = group.Sum(p => p.Sold),
                    TotalInStock = group.Sum(p => p.Instock),
                    TotalValueInStock = group.Sum(p => p.Instock * p.Price)
                })
                .ToList();

            var viewModel = new ProductStatisticsViewModel
            {
                Date = today,
                TotalProducts = totalProducts,
                TotalInStock = totalInStock,
                TotalSold = totalSold,
                TotalValueInStock = totalValueInStock,
                TopSellingProducts = topSelling,
                OutOfStockProducts = outOfStock,
                LowStockProducts = lowStock,
                ProductsByCategory = productsByCategory
            };

            return View(viewModel);
        }
    }
}
