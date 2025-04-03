using DietDoHongTran.Models;
using Microsoft.AspNetCore.Mvc;
using DietDoHongTran.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DietDoHongTran.Repositories;

namespace DietDoHongTran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoriesController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        public async Task<IActionResult> Display(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            // Lấy thông tin Category cần xóa
            var category = await _categoryRepository.GetByIdAsync(id);

            // Kiểm tra xem Category có tồn tại hay không
            if (category == null)
            {
                return NotFound();
            }

            // Trả về view để xác nhận xóa
            return View(category);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Lấy thông tin Category cần xóa
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category != null)
            {
                // Lấy danh sách sản phẩm liên quan đến Category
                var products = await _productRepository.GetProductsByCategoryIdAsync(id);

                foreach (var product in products)
                {
                    // Cập nhật CategoryId của sản phẩm thành null
                    product.CategoryId = null;
                    await _productRepository.UpdateAsync(product); // Lưu thay đổi vào cơ sở dữ liệu
                }

                // Sau khi cập nhật các sản phẩm, xóa Category
                await _categoryRepository.DeleteAsync(id);
            }

            // Chuyển hướng về trang danh sách Category
            return RedirectToAction(nameof(Index));
        }
    }
}
    
