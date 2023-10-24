using Microsoft.AspNetCore.Mvc;

namespace InfinityWeb.Controllers
{
    public class GroupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
