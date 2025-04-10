using DietDoHongTran.Models;
using Microsoft.EntityFrameworkCore;

namespace DietDoHongTran.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
            Console.WriteLine($"Database Context Initialized: {_context.Categories.Count()} categories loaded.");
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync (Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return; // Hoặc bạn có thể throw NotFoundException nếu muốn
            }

            // Cập nhật các sản phẩm liên quan và gán CategoryId thành null
            var products = await _context.Products.Where(p => p.CategoryId == id).ToListAsync();
            foreach (var product in products)
            {
                product.CategoryId = null;
                _context.Products.Update(product);  // Cập nhật lại các sản phẩm
            }

            await _context.SaveChangesAsync();  // Lưu thay đổi của các sản phẩm

            // Xóa Category
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                return new List<Product>();

            return await _context.Products
                .Where(p => productIds.Contains((int)p.Id))
                .ToListAsync();
        }
    }
}
