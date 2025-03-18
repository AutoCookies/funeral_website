using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DietDoHongTran.Models;
using Microsoft.AspNetCore.Authorization;
using DietDoHongTran.Repositories;

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

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
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
    }
}
