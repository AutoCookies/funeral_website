using Microsoft.AspNetCore.Mvc;

namespace DietDoHongTran.Controllers
{
    public class IntroduceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
