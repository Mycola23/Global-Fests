using Microsoft.AspNetCore.Mvc;

namespace GlobalFests.Controllers
{
    public class WorldMapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
