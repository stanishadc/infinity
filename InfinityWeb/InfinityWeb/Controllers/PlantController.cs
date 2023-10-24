using Microsoft.AspNetCore.Mvc;

namespace InfinityWeb.Controllers
{
    public class PlantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
