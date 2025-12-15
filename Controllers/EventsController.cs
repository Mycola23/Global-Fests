using GlobalFests.Services;
using GlobalFests.EFModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            string? title,
            string? city,
            int? countryId,
            int? typeId,
            decimal? minPrice,
            decimal? maxPrice,
            DateTime? startDateFrom,
            DateTime? startDateTo,
            DateTime? cursorDate,
            int? cursorId,
            int pageSize = 15)
        {
            ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
            ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();

            var result = await _eventService.SearchEventsAsync(
                title,
                city,
                countryId,
                typeId,
                minPrice,
                maxPrice,
                startDateFrom,
                startDateTo,
                true, // approved = true для публічного перегляду
                cursorDate,
                cursorId,
                pageSize);

            // Зберігаємо параметри фільтрації для ViewBag
            ViewBag.Title = title;
            ViewBag.City = city;
            ViewBag.CountryId = countryId;
            ViewBag.TypeId = typeId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.StartDateFrom = startDateFrom;
            ViewBag.StartDateTo = startDateTo;

            return View(result);
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
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Events/Create" });

            ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
            ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();
            ViewBag.Genres = await _lookupService.GetAllGenresAsync();

            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventEntity)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            // Отримуємо UserId з поточного користувача
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);
            eventEntity.OrganizerId = userId;

            // Видаляємо помилки валідації для навігаційних властивостей
            // Вони приходять з форми як ID, а не як об'єкти
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

                ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
                ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();
                ViewBag.Genres = await _lookupService.GetAllGenresAsync();
                return View(eventEntity);
            }

            try
            {
                await _eventService.CreateEventAsync(eventEntity);
                TempData["SuccessMessage"] = "Event created successfully! It will be visible after approval.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating event: {ex.Message}");
                ViewBag.Countries = await _lookupService.GetAllCountriesAsync();
                ViewBag.EventTypes = await _lookupService.GetAllEventTypesAsync();
                ViewBag.Genres = await _lookupService.GetAllGenresAsync();
                return View(eventEntity);
            }
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event eventEntity)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            if (id != eventEntity.Id)
                return BadRequest();

            // Видаляємо помилки валідації для навігаційних властивостей
            ModelState.Remove("Country");
            ModelState.Remove("Type");
            ModelState.Remove("Organizer");
            ModelState.Remove("Tickets");
            ModelState.Remove("Genres");
            ModelState.Remove("Performers");
            ModelState.Remove("WishList");

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

            // Отримуємо всі події користувача (включаючи неопубліковані)
            var result = await _eventService.SearchEventsAsync(
                title: null,
                city: null,
                countryId: null,
                typeId: null,
                minPrice: null,
                maxPrice: null,
                startDateFrom: null,
                startDateTo: null,
                approved: null, // Показуємо всі події (approved і не approved)
                cursorDate: null,
                cursorId: null,
                pageSize: 100);

            return View(result);
        }
    }
}