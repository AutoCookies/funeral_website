using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using week2_2280602627.Models;
using week2_2280602627.Repositories; // Đảm bảo thêm namespace chứa IProductRepository

namespace week2_2280602627.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;

        // Inject IProductRepository qua constructor
        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            // Lấy danh sách sản phẩm từ repository
            var products = _productRepository.GetAll();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
