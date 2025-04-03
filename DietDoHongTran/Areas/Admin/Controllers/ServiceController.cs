using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DietDoHongTran.Models;
using Microsoft.AspNetCore.Authorization;
using DietDoHongTran.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DietDoHongTran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IProductRepository _productRepository;

        public ServiceController(IServiceRepository serviceRepository, IProductRepository productRepository)
        {
            _serviceRepository = serviceRepository;
            _productRepository = productRepository;
        }

        // Hiển thị danh sách dịch vụ
        public async Task<IActionResult> Index()
        {
            var services = await _serviceRepository.GetAllAsync();
            return View(services);
        }

        public async Task<IActionResult> Add()
        {
            ViewBag.Products = new SelectList(await _productRepository.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Service model, List<int> ProductIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = new SelectList(await _productRepository.GetAllAsync(), "Id", "Name");
                return View(model);
            }

            await _serviceRepository.AddAsync(model, ProductIds);
            return RedirectToAction(nameof(Index));
        }

        // Hiển thị trang chỉnh sửa dịch vụ
        public async Task<IActionResult> Update(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var allProducts = await _productRepository.GetAllAsync();
            ViewBag.Products = allProducts.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name,
                Selected = service.ServiceProducts?.Any(sp => sp.ProductId == p.Id) ?? false
            }).ToList();

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Service model, List<int> ProductIds)
        {
            if (!ModelState.IsValid)
            {
                var allProducts = await _productRepository.GetAllAsync();
                ViewBag.Products = allProducts.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = model.ServiceProducts?.Any(sp => sp.ProductId == p.Id) ?? false
                }).ToList();
                return View(model);
            }

            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            await _serviceRepository.UpdateAsync(model, ProductIds);
            return RedirectToAction(nameof(Index));
        }

        // Xác nhận xóa dịch vụ
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            await _serviceRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
