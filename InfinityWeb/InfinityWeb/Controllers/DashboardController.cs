using Microsoft.AspNetCore.Mvc;

namespace InfinityWeb.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
