using Microsoft.AspNetCore.Mvc;

namespace DietDoHongTran.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
