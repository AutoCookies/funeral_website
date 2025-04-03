using DietDoHongTran.Models;
using DietDoHongTran.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DietDoHongTran.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories
            .Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                // Gán Href sau khi Id đã có giá trị
                Href = $"/Product/CategoryProducts?categoryId={c.Id}"
            })
            .ToListAsync();

            return View(categories);
        }
    }
}
