using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using week2_2280602627.Models;
using Microsoft.AspNetCore.Hosting;
using week2_2280602627.Repositories;
using System.IO;
using System;
using System.Threading.Tasks;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
    {
        _productRepository = productRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.Categories = new SelectList(new MockCategoryRepository().GetAllCategories(), "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Product product, IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            product.ImageUrl = await SaveImage(imageFile);
        }
        else
        {
            product.ImageUrl = "/images/default.jpg"; // Hình mặc định nếu không có ảnh
        }

        _productRepository.Add(product);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Index()
    {
        var products = _productRepository.GetAll();
        return View(products);
    }

    public IActionResult Display(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    public IActionResult Update(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    public IActionResult Update(Product product)
    {
        if (ModelState.IsValid)
        {
            _productRepository.Update(product);
            return RedirectToAction("Index");
        }
        return View(product);
    }

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

    private async Task<string> SaveImage(IFormFile image)
    {
        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return "/uploads/" + uniqueFileName;
    }

    public IActionResult Details(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    public IActionResult Edit(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product product, IFormFile imageFile)
    {
        var existingProduct = _productRepository.GetById(product.Id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        // Cập nhật thông tin sản phẩm
        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.Description;
        existingProduct.CategoryId = product.CategoryId;

        // Xử lý cập nhật ảnh
        if (imageFile != null && imageFile.Length > 0)
        {
            existingProduct.ImageUrl = await SaveImage(imageFile);
        }

        _productRepository.Update(existingProduct);
        return RedirectToAction("Index");
    }
}
