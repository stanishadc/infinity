using Microsoft.AspNetCore.Mvc;

namespace InfinityWeb.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
