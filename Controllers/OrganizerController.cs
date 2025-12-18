using Microsoft.AspNetCore.Mvc;

namespace GlobalFests.Controllers
{
    public class OrganizerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
