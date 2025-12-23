using System.Security.Claims;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Repositories;
using GlobalFests.Services;
using GlobalFests.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Controllers
{
    public class OrganizerController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILookupService _lookupService;
        private readonly IPerformerRepository _performerRepo;
        private readonly IEventRepository _eventsRepo;
        public OrganizerController(IUserService userService, ILookupService lookupService, IPerformerRepository performerRepo,IEventRepository eventsRepo)
        {
            _userService = userService;
            _lookupService = lookupService;
            _performerRepo = performerRepo;
            _eventsRepo = eventsRepo;
        }
        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);
            var model = new OrganizerPanelViewModel {
                Performers = await _performerRepo.GetAllPerformersByOrganizerAsync(userId,null,null,10),
                Events = await _eventsRepo.GetAllEventsByOrganizerAsync(userId,null,null,10),
            };
            

            return View(model);
        }


        public async Task<IActionResult> CreatePerformer()
        {
            var model = new CreatePerformerViewModel
            {
                Countries = await _lookupService.GetAllCountriesAsync(),
                Genres = await _lookupService.GetAllGenresAsync(), 
                NewPerformer = new Performer()
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePerformerViewModel model)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);
            model.NewPerformer.CreatedBy = userId;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                model.Countries = await _lookupService.GetAllCountriesAsync();
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
                            model.NewPerformer.Genres.Add(genre);
                        }
                    }
                }

                await _performerRepo.CreateAsync(model.NewPerformer);
        
                TempData["SuccessMessage"] = "Event created successfully! It will be visible after approval.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating event: {ex.Message}");
                model.Countries = await _lookupService.GetAllCountriesAsync();
                model.Genres = await _lookupService.GetAllGenresAsync();
                return View(model);
            }
        }

        // GET: Organizer/EditPerformer/5
        public async Task<IActionResult> EditPerformer(int id)
        {
            // Отримуємо перформера з усіма зв'язками
            var performer = await _performerRepo.GetByIdAsync(id);

            if (performer == null)
                return NotFound();

            // Перевірка прав доступу (тільки власник або адмін)
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (performer.CreatedBy != userId && !User.IsInRole("Admin"))
                return Forbid();

            var model = new EditPerformerViewModel
            {
                Performer = performer,
                Countries = await _lookupService.GetAllCountriesAsync(),
                Genres = await _lookupService.GetAllGenresAsync(),
                SelectedGenreIds = performer.Genres.Select(g => g.Id).ToList()
            };

            return View(model);
        }

        // POST: Organizer/EditPerformer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPerformer(int id, EditPerformerViewModel model)
        {
            if (id != model.Performer.Id)
                return BadRequest();

            // Видаляємо навігаційні властивості з валідації
            ModelState.Remove("Performer.Country");
            ModelState.Remove("Performer.Creator");
            ModelState.Remove("Performer.Genres");
            ModelState.Remove("Performer.Events");

            if (!ModelState.IsValid)
            {
                model.Countries = await _lookupService.GetAllCountriesAsync();
                model.Genres = await _lookupService.GetAllGenresAsync();
                return View(model);
            }

            try
            {
                // Отримуємо існуючий об'єкт з бази (з відстеженням для оновлення зв'язків)
                var existingPerformer = await _performerRepo.GetByIdAsync(id);
                if (existingPerformer == null) return NotFound();

                // Оновлюємо основні поля
                existingPerformer.Name = model.Performer.Name;
                existingPerformer.Description = model.Performer.Description;
                existingPerformer.CountryId = model.Performer.CountryId;
                existingPerformer.Avatar = model.Performer.Avatar;
                // existingPerformer.Genre = ... (старе текстове поле можна залишити пустим або оновлювати)

                // Оновлюємо жанри (Many-to-Many)
                existingPerformer.Genres.Clear();
                if (model.SelectedGenreIds != null && model.SelectedGenreIds.Any())
                {
                    foreach (var genreId in model.SelectedGenreIds)
                    {
                        var genre = await _lookupService.GetGenreByIdAsync(genreId);
                        if (genre != null)
                        {
                            existingPerformer.Genres.Add(genre);
                        }
                    }
                }

                await _performerRepo.UpdateAsync(existingPerformer);

                TempData["SuccessMessage"] = "Performer updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating performer: {ex.Message}");
                model.Countries = await _lookupService.GetAllCountriesAsync();
                model.Genres = await _lookupService.GetAllGenresAsync();
                return View(model);
            }
        }
    }
}
