using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using week2_2280602627.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public IActionResult Add()
    {
        // Lấy danh sách các danh mục
        var categories = _categoryRepository.GetAllCategories();
        if (categories == null || !categories.Any())
        {
            ViewBag.Message = "Không có danh mục nào trong hệ thống.";
        }

        // Chuyển danh sách danh mục vào ViewBag để sử dụng trong dropdown
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Product product, List<IFormFile> imageFiles)
    {
        if (ModelState.IsValid)
        {
            // Xử lý các hình ảnh và lưu URL vào ImageUrls
            product.ImageUrls = new List<string>();

            // Kiểm tra xem người dùng có cung cấp URL hình ảnh không
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                product.ImageUrls.Add(product.ImageUrl); // Dùng URL hình ảnh mà người dùng cung cấp
            }

            foreach (var imageFile in imageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageUrl = await SaveImage(imageFile); // Hàm SaveImage sẽ lưu ảnh vào thư mục và trả về URL
                    product.ImageUrls.Add(imageUrl);
                }
            }

            // Kiểm tra CategoryId (nếu chưa chọn, có thể thiết lập mặc định là null hoặc thông báo lỗi)
            if (product.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Vui lòng chọn một danh mục.");
            }

            // Lưu sản phẩm vào repository
            if (ModelState.IsValid)
            {
                _productRepository.Add(product);
                return RedirectToAction("Index");
            }
        }

        // Nếu có lỗi, trả lại danh sách danh mục và tiếp tục hiển thị form
        var categories = _categoryRepository.GetAllCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(product);
    }

    public IActionResult Index()
    {
        // Assuming you have a Category model and _categoryRepository
        var categories = _categoryRepository.GetAllCategories()
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

        ViewBag.Categories = categories; // Pass the categories to ViewBag

        var products = _productRepository.GetAll(); // Fetch products
        return View(products);
    }



    public IActionResult Details(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }

        // Lấy danh mục của sản phẩm từ repository
        var category = _categoryRepository.GetById(product.CategoryId);
        if (category != null)
        {
            // Gán tên danh mục vào ViewBag
            ViewBag.CategoryName = category.Name;
        }

        return View(product);
    }


    // Xử lý sửa sản phẩm
    public IActionResult Edit(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }

        // Lấy danh mục và gán vào ViewBag
        ViewBag.Categories = new SelectList(_categoryRepository.GetAllCategories(), "Id", "Name");
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _productRepository.Update(product);
            return RedirectToAction("Index");
        }
        return View(product);
    }

    // Xử lý xóa sản phẩm
    public IActionResult Delete(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        _productRepository.Delete(id);
        return RedirectToAction("Index");
    }

    // Hàm xử lý lưu hình ảnh vào thư mục và trả về URL
    private async Task<string> SaveImage(IFormFile imageFile)
    {
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        string filePath = Path.Combine(uploadsFolder, fileName);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return "/uploads/" + fileName;
    }
}
