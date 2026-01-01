using System.Security.Claims;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Helpers;
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
        private readonly IOrganizerStatsService _organizerStats;
        public OrganizerController(IUserService userService, ILookupService lookupService, IPerformerRepository performerRepo,IEventRepository eventsRepo, IOrganizerStatsService organizerStats)
        {
            _userService = userService;
            _lookupService = lookupService;
            _performerRepo = performerRepo;
            _eventsRepo = eventsRepo;
            _organizerStats = organizerStats;
        }
        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);
            var model = new OrganizerPanelViewModel
            {
                Performers = await _performerRepo.GetAllPerformersByOrganizerAsync(userId, null, null, 10),
                Events = await _eventsRepo.GetAllEventsByOrganizerAsync(userId, null, null, 10),
                Stats = await _organizerStats.GetOrganizerAllStats(userId)
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
                model.NewPerformer.Status = (int)Status.Pending;
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
           
            var performer = await _performerRepo.GetByIdAsync(id);

            if (performer == null)
                return NotFound();

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
               
                var existingPerformer = await _performerRepo.GetByIdAsync(id);
                if (existingPerformer == null) return NotFound();

               
                existingPerformer.Name = model.Performer.Name;
                existingPerformer.Description = model.Performer.Description;
                existingPerformer.CountryId = model.Performer.CountryId;
                existingPerformer.Avatar = model.Performer.Avatar;
                existingPerformer.Status = (int)Status.Pending;

                // think about else method of checking gow to update genreses,performers... like for events as for performers
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

        public async Task<IActionResult> DetailsPerformer(int id)
        {
            var performerDto = await _performerRepo.GetPerformerWithDetailsAsync(id);

            if (performerDto == null)
                return NotFound();

            var model = new PerformerDetailsViewModel
            {
                Performer = performerDto
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePerformer(int id)
        {
            var performer = await _performerRepo.GetByIdAsync(id);

            if (performer == null)
            {
                TempData["ErrorMessage"] = "Performer not found.";
                return RedirectToAction(nameof(Index));
            }

           
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

          
            if (performer.CreatedBy != userId && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }

           
            try
            {
                bool isDeleted = await _performerRepo.DeleteAsync(id);

                if (isDeleted)
                {
                    TempData["SuccessMessage"] = "Performer deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Could not delete performer. Database error.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Cannot delete performer because they are assigned to events.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
