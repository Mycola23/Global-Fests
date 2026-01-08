using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GlobalFests.Models;
using GlobalFests.Services;
using GlobalFests.Helpers;
using GlobalFests.DTOs;

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
        
        var events = await _eventService.SearchEventsAsync<EventDto>(
            title: null,
            city: null,
            countryId: null,
            typeId: null,
            minPrice: null,
            maxPrice: null,
            startDateFrom: null,
            startDateTo: null,
            status: (int)Status.Approved,
            cursorDate: null,
            cursorId: null,
            pageSize: 12);

        return View(events);
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
