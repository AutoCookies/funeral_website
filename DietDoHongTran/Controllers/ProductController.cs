using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DietDoHongTran.Models;
using Microsoft.AspNetCore.Authorization;
using DietDoHongTran.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DietDoHongTran.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Display(int id, string returnUrl = null)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Product"); // Use returnUrl if provided, otherwise default to Product/Index
            return View(product);
        }

        public async Task<IActionResult> CategoryList()
        {
            var categories = await _categoryRepository.GetAllAsync();

            // Kiểm tra dữ liệu trước khi truyền vào View
            if (categories == null || !categories.Any())
            {
                return BadRequest("Không có danh mục nào được tìm thấy.");
            }

            return View(categories);
        }

        public async Task<IActionResult> CategoryProducts(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);

            // Truyền dữ liệu cần thiết đến View
            ViewBag.CategoryName = category.Name;
            return View(products); // Trả về View cùng với danh sách sản phẩm
        }
    }
}
