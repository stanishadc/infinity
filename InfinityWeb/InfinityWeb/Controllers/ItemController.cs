using Microsoft.AspNetCore.Mvc;

namespace InfinityWeb.Controllers
{
    public class ItemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
