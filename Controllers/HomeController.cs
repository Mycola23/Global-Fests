using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GlobalFests.Models;
using GlobalFests.Services;
using GlobalFests.Helpers;
using GlobalFests.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Localization;

namespace GlobalFests.Controllers;

public class HomeController : Controller
{
    private readonly IEventService _eventService;

    public HomeController(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        int? userId = null;
        if (User.Identity.IsAuthenticated)
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null) userId = int.Parse(claimId.Value);
        }

        var model = await _eventService.GetHomePageDataAsync(userId);
        return View(model);
    }
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }


}
