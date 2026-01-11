using GlobalFests.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFests.Controllers
{
    [Authorize]
    public class WishListController : Controller
    {
        private readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        // WishList Page
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var events = await _wishListService.GetUserWishListAsync(userId);
            return View(events);
        }

        // Toggke Event
        [HttpPost]
        public async Task<IActionResult> Toggle(int eventId, string returnUrl)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _wishListService.ToggleWishListAsync(userId, eventId);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index");
        }
    }
}
