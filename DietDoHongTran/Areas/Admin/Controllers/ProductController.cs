using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DietDoHongTran.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DietDoHongTran.Repositories;

namespace DietDoHongTran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IServiceRepository _serviceRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IServiceRepository serviceRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();

            if (!categories.Any())
            {
                Console.WriteLine("No categories found!");
            }

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                return View(product);
            }

            // Lưu hình ảnh nếu người dùng chọn ảnh mới
            if (imageFile != null)
            {
                var imagePath = await SaveImage(imageFile);
                if (imagePath == null)
                {
                    ModelState.AddModelError("ImageUrl", "Lỗi khi tải lên ảnh.");
                    ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                    return View(product);
                }
                product.ImageUrl = imagePath;
            }

            await _productRepository.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveImage(IFormFile? image)
        {
            if (image == null) return null;

            try
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                    Console.WriteLine("Created directory: " + uploadFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                var savePath = Path.Combine(uploadFolder, uniqueFileName);

                Console.WriteLine("Saving image to: " + savePath);

                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                Console.WriteLine("Image saved successfully!");

                return "/images/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
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

        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var newImagePath = await SaveImage(imageFile);
                    if (newImagePath == null)
                    {
                        ModelState.AddModelError("ImageUrl", "Lỗi khi lưu hình ảnh.");
                        return View(product);
                    }
                    existingProduct.ImageUrl = newImagePath;
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;

                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                await _productRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể xóa sản phẩm, có thể sản phẩm đang được sử dụng.");
                return View("Delete", product);
            }
        }
    }
}
