using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFests.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateReviewDto model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _reviewService.AddReviewAsync(userId, model);
            return RedirectToAction("Details", "Events", new { id = model.EventId });
        }
    }
}
