using GlobalFests.Services;
using GlobalFests.EFModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GlobalFests.ViewModels;
using GlobalFests.Repositories;
using Microsoft.EntityFrameworkCore;
using GlobalFests.Helpers;
using GlobalFests.DTOs;

namespace GlobalFests.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ILookupService _lookupService;
        private readonly IEventRepository _eventRepo;

        public EventsController(
            IEventService eventService,
            ILookupService lookupService, IEventRepository eventRepo)
        {
            _eventService = eventService;
            _lookupService = lookupService;
            _eventRepo = eventRepo;
        }

        // GET: Events
        public async Task<IActionResult> Index(
            EventsViewModel model, DateTime? cursorDate, int? cursorId)
        {
            if (model.Search == null)
            {
                model.Search = new EventsSearchModel();
            }
            
            
            model.EventTypes = await _lookupService.GetAllEventTypesAsync();
            model.Countries = await _lookupService.GetAllCountriesAsync();
            model.Genres = await _lookupService.GetAllGenresAsync();

            
            var searchResult = await _eventService.SearchEventsAsync<EventDto>(
                model.Search.Title,
                model.Search.City,
                model.Search.CountryId,
                model.Search.TypeId,
                model.Search.MinPrice,
                model.Search.MaxPrice,
                model.Search.StartDateFrom,
                model.Search.StartDateTo,
            //      true, // approved = true
                (int)Status.Approved,
                cursorDate,
                cursorId,
                15 // pageSize
            );

            
            model.Events = searchResult;

            return View(model);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var eventDetails = await _eventService.GetEventWithDetailsAsync(id);

            if (eventDetails == null)
                return NotFound();
            var model = new EventDetailsViewModel
            {
                Event = eventDetails
            };

            return View(model);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {

            var now = DateTime.Now;
            var startDateClean = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            var model = new CreateEventsViewModel
            {
                NewEvent = new Event
                {
                    StartDate = startDateClean,
                    EndDate = startDateClean.AddHours(2),
                },
                Countries = await _lookupService.GetAllCountriesAsync(),
                EventTypes = await _lookupService.GetAllEventTypesAsync(),
                Genres = await _lookupService.GetAllGenresAsync(),
            };
            

            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Events/Create" });

            return View(model);
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventsViewModel model, string submitAction)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);
            model.NewEvent.OrganizerId = userId;
            if (submitAction == "Draft")
            {
                model.NewEvent.Status = (int)Status.Draft; // Status 0
            }
            else
            {
                model.NewEvent.Status = (int)Status.Pending; // Status 1
            }


            ModelState.Remove("NewEvent.Country");
            ModelState.Remove("NewEvent.Type");
            ModelState.Remove("NewEvent.Organizer");
            ModelState.Remove("NewEvent.Tickets");
            ModelState.Remove("NewEvent.Performers");
            ModelState.Remove("NewEvent.WishList");
            ModelState.Remove("submitAction");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                model.Countries = await _lookupService.GetAllCountriesAsync();
                model.EventTypes = await _lookupService.GetAllEventTypesAsync();
                model.Genres = await _lookupService.GetAllGenresAsync();
                return View(model);
            }

            try
            {

                if (model.SelectedGenreIds != null && model.SelectedGenreIds.Any())
                {
                    foreach (var genreId in model.SelectedGenreIds)
                    {

                        var genre = await _lookupService.GetGenreByIdAsync(genreId);
                        if (genre != null)
                        {
                            model.NewEvent.Genres.Add(genre);
                        }
                    }
                }

                await _eventService.CreateEventAsync(model.NewEvent);
                TempData["SuccessMessage"] = "Event created successfully! It will be visible after approval.";
                if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Events", "Admin");
                }
                return RedirectToAction("Index", "Organizer");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating event: {ex.Message}");
                model.Countries = await _lookupService.GetAllCountriesAsync();
                model.EventTypes = await _lookupService.GetAllEventTypesAsync();
                model.Genres = await _lookupService.GetAllGenresAsync();
                return View(model);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(id);
            

            if (eventEntity == null)
                return NotFound();

           
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (eventEntity.OrganizerId != userId &&  !User.IsInRole("SuperAdmin") && !User.IsInRole("Admin"))
                return Forbid();

            var model = new EditEventsViewModel
            {
                Event = eventEntity,
                Countries = await _lookupService.GetAllCountriesAsync(),
                EventTypes = await _lookupService.GetAllEventTypesAsync(),
                Genres = await _lookupService.GetAllGenresAsync(),

                SelectedGenreIds = eventEntity.Genres.Select(g => g.Id).ToList()
            };

            return View(model);
        }

        // POST: Events/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEventsViewModel model)
        {
            if (id != model.Event.Id)
                return BadRequest();

            
            ModelState.Remove("Event.Country");
            ModelState.Remove("Event.Type");
            ModelState.Remove("Event.Organizer");
            ModelState.Remove("Event.Genres");
            ModelState.Remove("Event.Performers");

            if (!ModelState.IsValid)
            {
                model.Countries = await _lookupService.GetAllCountriesAsync();
                model.EventTypes = await _lookupService.GetAllEventTypesAsync();
                model.Genres = await _lookupService.GetAllGenresAsync();
                return View(model);
            }

            try
            {
              
                var existingEvent = await _eventRepo.GetByIdAsync(id);
                if (existingEvent == null) return NotFound();

                existingEvent.Title = model.Event.Title;
                existingEvent.Description = model.Event.Description;
                existingEvent.StartDate = model.Event.StartDate;
                existingEvent.EndDate = model.Event.EndDate;
                existingEvent.TypeId = model.Event.TypeId;
                existingEvent.CountryId = model.Event.CountryId;
                existingEvent.City = model.Event.City;
                existingEvent.Address = model.Event.Address;
                existingEvent.TicketPrice = model.Event.TicketPrice;
                existingEvent.TicketAmount = model.Event.TicketAmount;
                existingEvent.Poster = model.Event.Poster;
                existingEvent.Latitude = model.Event.Latitude;
                existingEvent.Longitude = model.Event.Longitude;

               
                existingEvent.Genres.Clear(); 
                if (model.SelectedGenreIds != null && model.SelectedGenreIds.Any())
                {
                    foreach (var genreId in model.SelectedGenreIds)
                    {
                        var genre = await _lookupService.GetGenreByIdAsync(genreId);
                        if (genre != null) existingEvent.Genres.Add(genre);
                    }
                }

                
                await _eventRepo.UpdateAsync(existingEvent);

                TempData["SuccessMessage"] = "Event updated successfully!";
                if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Events", "Admin");
                }
                return RedirectToAction("Index", "Organizer");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating event: {ex.Message}");
                model.Countries = await _lookupService.GetAllCountriesAsync();
                model.EventTypes = await _lookupService.GetAllEventTypesAsync();
                model.Genres = await _lookupService.GetAllGenresAsync();
                return View(model);
            }
        }
        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            var eventDetails = await _eventService.GetEventWithDetailsAsync(id);

            if (eventDetails == null)
                return NotFound();
            
            return View(eventDetails);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            try
            {
                var deleted = await _eventService.DeleteEventAsync(id);
                if (deleted)
                {
                    TempData["SuccessMessage"] = "Event deleted successfully!";
                    if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                    {
                        return RedirectToAction("Events", "Admin");
                    }
                    return RedirectToAction("Index", "Organizer");
                }
                else
                {
                    TempData["ErrorMessage"] = "Event not found.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting event: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: Events/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(userIdClaim.Value);

            var result = await _eventService.SearchEventsAsync<EventDto>(
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
                pageSize: 100);

            return View(result);
        }
    }
}