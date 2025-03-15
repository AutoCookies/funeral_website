using week2_2280602627.Models;
using System.Collections.Generic;
using System.Linq;

public class MockProductRepository : IProductRepository
{
    private readonly List<Product> _products;
    private readonly List<Category> _categories;

    public MockProductRepository()
    {
        // Khởi tạo danh mục mẫu
        _categories = new List<Category>
        {
            new Category { Id = 1, Name = "Quần Áo" },
            new Category { Id = 2, Name = "Giày Dép" }
        };

        // Khởi tạo danh sách sản phẩm mẫu
        _products = new List<Product>{};
    }

    public IEnumerable<Product> GetAll()
    {
        return _products ?? new List<Product>();
    }

    public Product GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product product)
    {
        if (_products.Any())
        {
            product.Id = _products.Max(p => p.Id) + 1;
        }
        else
        {
            product.Id = 1;
        }

        _products.Add(product);
    }

    public void Update(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            _products[index] = product;
        }
    }

    public void Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
        }
    }

    // Lấy danh mục theo Id
    public Category GetCategoryById(int id)
    {
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    // Lấy tất cả danh mục
    public IEnumerable<Category> GetAllCategories()
    {
        return _categories;
    }
}
