using GlobalFests.DTOs;
using GlobalFests.Helpers;
using GlobalFests.Repositories;
using GlobalFests.Services;
using GlobalFests.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFests.Controllers
{
    public class WorldMapController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ILookupService _lookupService;
        private readonly IEventRepository _eventRepo;

        public WorldMapController(
            IEventService eventService,
            ILookupService lookupService, IEventRepository eventRepo)
        {
            _eventService = eventService;
            _lookupService = lookupService;
            _eventRepo = eventRepo;
        }
        public async Task<IActionResult> Index()
        {
            var model = new WorldMapViewModel
            {
                Countries = await _lookupService.GetAllCountriesAsync(),
                EventTypes = await _lookupService.GetAllEventTypesAsync(),
                Genres = await _lookupService.GetAllGenresAsync()
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetMapEvents([FromQuery] EventsSearchModel search)
        {
            
            var result = await _eventService.SearchEventsAsync<EventWorldMapDto>(
                search.Title, search.City, search.CountryId, search.TypeId, search.GenreId,
                search.MinPrice, search.MaxPrice, search.StartDateFrom,
                search.StartDateTo, (int)Status.Approved, null, null, pageSize: 200);

            return Json(result.Items);
        }

        
    }
}
