﻿using DietDoHongTran.Models;

namespace DietDoHongTran.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync (Product product);
        Task UpdateAsync (Product product);
        Task DeleteAsync (int id);
        Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task<List<Product>> GetProductsByIdsAsync(List<int> productIds);
    }
}
