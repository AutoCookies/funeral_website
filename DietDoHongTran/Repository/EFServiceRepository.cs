using DietDoHongTran.Models;
using DietDoHongTran.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietDoHongTran.Repositories
{
    public class EFServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public EFServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceProducts)
                .ThenInclude(sp => sp.Product)
                .ToListAsync();
        }

        public async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.ServiceProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Service service, List<int> productIds)
        {
            // Lưu service vào database trước để có ID
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            // Lấy danh sách sản phẩm từ DB
            var selectedProducts = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Tạo danh sách ServiceProduct với ServiceId chính xác
            service.ServiceProducts = selectedProducts
                .Select(p => new ServiceProduct { ServiceId = service.Id, ProductId = p.Id })
                .ToList();

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Service service, List<int> productIds)
        {
            var existingService = await _context.Services
                .Include(s => s.ServiceProducts)
                .FirstOrDefaultAsync(s => s.Id == service.Id);

            if (existingService == null)
            {
                throw new Exception("Service not found.");
            }

            // Cập nhật thông tin dịch vụ
            existingService.Name = service.Name;
            existingService.Description = service.Description;
            existingService.BasePrice = service.BasePrice;

            // Xóa toàn bộ ServiceProduct cũ trước khi thêm mới
            await RemoveServiceProductsAsync(existingService.ServiceProducts);

            var selectedProducts = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            existingService.ServiceProducts = selectedProducts
                .Select(p => new ServiceProduct { ServiceId = service.Id, ProductId = p.Id })
                .ToList();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.Services
                .Include(s => s.ServiceProducts)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                throw new Exception("Service not found.");
            }

            // Xóa liên kết ServiceProducts trước khi xóa Service
            await RemoveServiceProductsAsync(service.ServiceProducts);
            _context.Services.Remove(service);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
        {
            return await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public async Task RemoveServiceProductsAsync(List<ServiceProduct> serviceProducts)
        {
            if (serviceProducts.Any())
            {
                _context.ServiceProducts.RemoveRange(serviceProducts);
                await _context.SaveChangesAsync();
            }
        }
    }
}
