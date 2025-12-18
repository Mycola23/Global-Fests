using GlobalFests.Services;
using GlobalFests.EFModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GlobalFests.ViewModels;

namespace GlobalFests.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ILookupService _lookupService;

        public EventsController(
            IEventService eventService,
            ILookupService lookupService)
        {
            _eventService = eventService;
            _lookupService = lookupService;
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

            
            var searchResult = await _eventService.SearchEventsAsync(
                model.Search.Title,
                model.Search.City,
                model.Search.CountryId,
                model.Search.TypeId,
                model.Search.MinPrice,
                model.Search.MaxPrice,
                model.Search.StartDateFrom,
                model.Search.StartDateTo,
                true, // approved = true
                cursorDate,
                cursorId,
                15 // pageSize
            );

            // Assign result back to ViewModel
            model.Events = searchResult;

            return View(model);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var eventDetails = await _eventService.GetEventWithDetailsAsync(id);

            if (eventDetails == null)
                return NotFound();

            return View(eventDetails);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {

            var now = DateTime.Now;
            var startDateClean = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            var model = new CreateViewModel
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
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);
            model.NewEvent.OrganizerId = userId;

            
            ModelState.Remove("Country");
            ModelState.Remove("Type");
            ModelState.Remove("Organizer");
            ModelState.Remove("Tickets");
            ModelState.Remove("Genres");
            ModelState.Remove("Performers");
            ModelState.Remove("WishList");

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
                await _eventService.CreateEventAsync(model.NewEvent);
                TempData["SuccessMessage"] = "Event created successfully! It will be visible after approval.";
                return RedirectToAction(nameof(Index));
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
        //fix all viewbag remove completely
        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event eventEntity)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            if (id != eventEntity.Id)
                return BadRequest();

           
           
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
                ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();
                ViewBag.Genres = await _lookupService.GetAllGenresAsync();
                return View(eventEntity);
            }

            try
            {
                await _eventService.UpdateEventAsync(eventEntity);
                TempData["SuccessMessage"] = "Event updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating event: {ex.Message}");
                ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
                ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();
                ViewBag.Genres = await _lookupService.GetAllGenresAsync();
                return View(eventEntity);
            }
        }

        //// POST: Events/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Event eventEntity)
        //{
        //    if (!User.Identity?.IsAuthenticated ?? true)
        //        return RedirectToAction("Login", "Account");

        //    if (id != eventEntity.Id)
        //        return BadRequest();

        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
        //        ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();
        //        ViewBag.Genres = await _lookupService.GetAllGenresAsync();
        //        return View(eventEntity);
        //    }

        //    try
        //    {
        //        await _eventService.UpdateEventAsync(eventEntity);
        //        TempData["SuccessMessage"] = "Event updated successfully!";
        //        return RedirectToAction(nameof(Details), new { id });
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", $"Error updating event: {ex.Message}");
        //        ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
        //        ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();
        //        ViewBag.Genres = await _lookupService.GetAllGenresAsync();
        //        return View(eventEntity);
        //    }
        //}

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
                    return RedirectToAction(nameof(Index));
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

            var result = await _eventService.SearchEventsAsync(
                title: null,
                city: null,
                countryId: null,
                typeId: null,
                minPrice: null,
                maxPrice: null,
                startDateFrom: null,
                startDateTo: null,
                approved: null, 
                cursorDate: null,
                cursorId: null,
                pageSize: 100);

            return View(result);
        }
    }
}