using DietDoHongTran.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DietDoHongTran.Repositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service> GetByIdAsync(int id);
        Task AddAsync(Service service, List<int> productIds);
        Task UpdateAsync(Service service, List<int> productIds);
        Task DeleteAsync(int id);
        Task<List<Product>> GetProductsByIdsAsync(List<int> ids);
        Task RemoveServiceProductsAsync(List<ServiceProduct> serviceProducts);
    }
}
