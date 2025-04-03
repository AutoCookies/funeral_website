using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietDoHongTran.Models;

namespace DietDoHongTran.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
    }
}
