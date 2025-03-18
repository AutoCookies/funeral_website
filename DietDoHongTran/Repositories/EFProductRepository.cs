using DietDoHongTran.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietDoHongTran.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ServiceProducts) // Kiểm tra xem có liên kết nào không
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return;
            }

            // Nếu sản phẩm có liên kết với ServiceProducts, loại bỏ liên kết trước khi xóa
            if (product.ServiceProducts != null && product.ServiceProducts.Any())
            {
                _context.ServiceProducts.RemoveRange(product.ServiceProducts);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();
        }
    }
}
