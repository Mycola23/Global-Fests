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
        private readonly IWishListService _wishListService;
        private readonly IReviewService _reviewService;
        private readonly IPerformerRepository _performerRepo;

        public EventsController(
            IPerformerRepository performerRepo,
            IReviewService reviewService,
            IEventService eventService,
            ILookupService lookupService, IEventRepository eventRepo, IWishListService wishListService)
        {

            _reviewService = reviewService;
            _eventService = eventService;
            _lookupService = lookupService;
            _eventRepo = eventRepo;
            _wishListService = wishListService;
            _performerRepo = performerRepo;
        }

        
        public async Task<IActionResult> Index(
        EventsViewModel model,
        string? cursorValue, 
        int? cursorId)
        {
            if (model.Search == null)
            {
                model.Search = new EventsSearchModel();
            }
            await LoadEventViewData(model);

            var searchResult = await _eventRepo.SearchEventsSortedAsync<EventDto>(
                title: model.Search.Title,
                city: model.Search.City,
                countryId: model.Search.CountryId,
                typeId: model.Search.TypeId,
                genreId: model.Search.GenreId,
                minPrice: model.Search.MinPrice,
                maxPrice: model.Search.MaxPrice,
                startDateFrom: model.Search.StartDateFrom,
                startDateTo: model.Search.StartDateTo,
                status: (int)Status.Approved,
                sortOrder: model.Search.SortOrder, 
                cursorValue: cursorValue,         
                cursorId: cursorId,
                pageSize: 15
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
                Event = eventDetails,
                IsInWishList = false,
                Reviews = await _reviewService.GetEventReviewsAsync(id),
            };

            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    model.IsInWishList = await _wishListService.IsInWishListAsync(userId, id);
                }
            }

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
                Performers = await _performerRepo.SearchPerformersAsync(null,null,null,null,15),
            };
            await LoadEventViewData(model);

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
                model.NewEvent.Status = (int)Status.Draft; 
            }
            else
            {
                model.NewEvent.Status = (int)Status.Pending; 
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

                await LoadEventViewData(model);
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

                if (model.SelectedPerformerIds != null && model.SelectedPerformerIds.Any())
                {

                    var selectedPerformers = await _performerRepo.GetPerformersByIdsAsync(model.SelectedPerformerIds);
                    foreach (var performer in selectedPerformers)
                    {
                        model.NewEvent.Performers.Add(performer);
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
                await LoadEventViewData(model);
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
                SelectedPerformerIds = eventEntity.Performers.Select(p => p.Id).ToList(),
                SelectedGenreIds = eventEntity.Genres.Select(g => g.Id).ToList(),
                HasSoldTickets = await _eventService.HasTicketsAsync(id),
                IsEventInProgress = eventEntity.StartDate <= DateTime.Now && eventEntity.EndDate > DateTime.Now
            };
            await LoadEventViewData(model);
            return View(model);
        }

        // POST: Events/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEventsViewModel model)
        {
            if (id != model.Event.Id) return BadRequest();

            //clearing validation properties
            ModelState.Remove("Event.Country");
            ModelState.Remove("Event.Type");
            ModelState.Remove("Event.Organizer");
            ModelState.Remove("Event.Genres");
            ModelState.Remove("Event.Performers");

            if (!ModelState.IsValid)
            {
                await LoadEventViewData(model);
                return View(model);
            }

            try
            {
                bool isOrganizer = User.IsInRole("Organizer") && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin");

                await _eventService.UpdateEventFullAsync(id, model, isOrganizer);
                TempData["SuccessMessage"] = "Event updated successfully!";

                if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Events", "Admin");
                }
                return RedirectToAction("Index", "Organizer");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex) 
            {
                ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
                await LoadEventViewData(model);
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Organizer");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating event: {ex.Message}");
                await LoadEventViewData(model);
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPerformersJson(string? search, DateTime? cursorDate, int? cursorId)
        {
            var result = await _performerRepo.SearchPerformersAsync(search, (int)Status.Approved, cursorDate, cursorId, 10);
            return Json(result);
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
            bool hasSoldTickets = await _eventService.HasTicketsAsync(id);

            if (hasSoldTickets)
            {
                TempData["ErrorMessage"] = "Cannot delete event because tickets have already been sold.";

                
                if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Events", "Admin");
                }
                return RedirectToAction("Index", "Organizer");
            }
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
                if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Events", "Admin");
                }
                return RedirectToAction("Index", "Organizer");
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
                genreId: null,
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

        private async Task LoadEventViewData<T>(T model) where T : class
        {
            dynamic dynamicModel = model;
            dynamicModel.Countries = await _lookupService.GetAllCountriesAsync();
            dynamicModel.EventTypes = await _lookupService.GetAllEventTypesAsync();
            dynamicModel.Genres = await _lookupService.GetAllGenresAsync();
        }
    }
}