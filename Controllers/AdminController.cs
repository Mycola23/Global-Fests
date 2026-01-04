using System.Security;
using GlobalFests.Data;
using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Helpers;
using GlobalFests.Repositories;
using GlobalFests.Services;
using GlobalFests.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Controllers
{
    public class AdminController : Controller
    {
        private readonly GlobalFestsContext _context;
        private readonly AdminManageItemsService _adminManageItemsService;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        private readonly IPerformerRepository _performerRepo;
        public AdminController(GlobalFestsContext context, AdminManageItemsService adminManageItemsService,IUserService userService, IEventService eventService, IPerformerRepository performerRepo)
        {
            _context = context;
            _adminManageItemsService = adminManageItemsService;
            _userService = userService;
            _eventService = eventService;
            _performerRepo = performerRepo;
        }

        public async Task<IActionResult> Index()
        {
            
            var totalUsers = await _context.Users.CountAsync();
            var totalEvents = await _context.Events.CountAsync();
            var totalPerformers = await _context.Performers.CountAsync();

            
            var totalRevenue = await _context.Tickets.AnyAsync()
                ? await _context.Tickets.SumAsync(t => t.Price)
                : 0;

            // moderatin
            var pendingEvents = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Type)
                .Where(e => e.Status == (int)Status.Pending)
                .OrderBy(e => e.CreatedAt)
                .Take(10) //change it
                .ToListAsync();

            var pendingPerformers = await _context.Performers
                .Include(p => p.Genres)
                .Where(e => e.Status == (int)Status.Pending)
                .OrderBy(p => p.CreatedAt)
                .Take(10) //change it
                .ToListAsync();

            // graphics datas 

            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var rawUserStats = await _context.Users
                .Where(u => u.CreatedAt >= sixMonthsAgo)
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync(); 

            
            var userStats = rawUserStats.Select(x => new MonthlyMetric
            {
                Month = $"{x.Year}-{x.Month:D2}", 
                Count = x.Count
            }).ToList();


            
            var eventTypeStats = await _context.Events
                .Include(e => e.Type)
                .GroupBy(e => e.Type.Type)
                .Select(g => new CategoryMetric
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalEvents = totalEvents,
                TotalPerformers = totalPerformers,
                TotalRevenue = totalRevenue,
                PendingEvents = pendingEvents,
                PendingPerformers = pendingPerformers,
                ChartStats = new AdminChartStats
                {
                    UserRegistrations = userStats,
                    EventsByType = eventTypeStats
                }
            };

            return View(model);
        }

        // --- moderation crud  ---

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                eventItem.Status = (int)Status.Approved;
                eventItem.RejectionReason = null;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Event '{eventItem.Title}' approved.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEvent(int id, string reason)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                eventItem.Status = (int)Status.Rejected;
                eventItem.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : reason;
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = "Event rejected and removed.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePerformer(int id)
        {
            var performer = await _context.Performers.FindAsync(id);
            if (performer != null)
            {
                performer.Status = (int)Status.Approved;
                performer.RejectionReason = null;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Performer '{performer.Name}' approved.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPerformer(int id, string reason)
        {
            var performer = await _context.Performers.FindAsync(id);
            if (performer != null)
            {
                performer.Status = (int)Status.Rejected;
                performer.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : reason;
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = "Performer rejected.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Countries()
        {
           
            var countries = await _context.Countries
                .AsNoTracking()
                .Select(c => new CountryDto
                {
                    Id = c.Id,
                    Name = c.CountryName,
                    Code = c.CountryCode,
                    
                    EventsCount = c.Events.Count(),
                    UsersCount = c.Users.Count(),
                    PerformersCount =  c.Performers.Count(),
                })
                .OrderBy(c => c.Name) 
                .ToListAsync();

            return View(countries);
        }

        [HttpGet]
        public async Task<IActionResult> EditCountry(int? id)
        {
            if (id == null || id == 0)
                return View("CountryForm", new CountryFormViewModel());

            var entity = await _context.Countries.FindAsync(id);
            if (entity == null) return NotFound();

            var model = new CountryFormViewModel
            {
                Id = entity.Id,
                CountryName = entity.CountryName,
                CountryCode = entity.CountryCode
            };

            return View("CountryForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCountry(CountryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CountryForm", model);
            }

            if (model.Id == 0)
            {
               
                var newCountry = new Country
                {
                    CountryName = model.CountryName,
                    CountryCode = model.CountryCode
                };
                _context.Countries.Add(newCountry);
            }
            else
            {
               
                var entity = await _context.Countries.FindAsync(model.Id);
                if (entity == null) return NotFound();

                entity.CountryName = model.CountryName;
                entity.CountryCode = model.CountryCode;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Countries));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            
            bool isInUse = await _context.Users.AnyAsync(u => u.CountryId == id)
                        || await _context.Events.AnyAsync(e => e.CountryId == id)
                        || await _context.Performers.AnyAsync(p => p.CountryId == id);

            if (isInUse)
            {
               
                TempData["ErrorMessage"] = $"Не можна видалити '{country.CountryName}', оскільки до неї прив'язані користувачі або події.";
                return RedirectToAction(nameof(Countries));
            }

            
            try
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Країну успішно видалено.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Сталася помилка при видаленні.";
            }

            return RedirectToAction(nameof(Countries));
        }


        [HttpGet]
        public async Task<IActionResult> Manage(AdminManageItemType type)
        {
            var items = await _adminManageItemsService.GetAllAsync(type);
            ViewData["EntityType"] = type;

            return View("UniversalIndex", items);
        }

        [HttpGet]
        public async Task<IActionResult> EditLookup(AdminManageItemType type, int? id)
        {
            if (id == null || id == 0)
                return View("UniversalForm", new AdminManageItemViewModel { EntityType = type });

            var model = await _adminManageItemsService.GetByIdAsync(type, id.Value);
            if (model == null) return NotFound();

            return View("UniversalForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLookup(AdminManageItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View("UniversalForm", model);
            await _adminManageItemsService.SaveAsync(model);

            return RedirectToAction(nameof(Manage), new { type = model.EntityType });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLookup(AdminManageItemType type, int id)
        {
            await _adminManageItemsService.DeleteAsync(type, id);
            return RedirectToAction(nameof(Manage), new { type });
        }


        // manage users
        [HttpGet]
        public async Task<IActionResult> Users(string? searchTerm, DateTime? cursorDate, int? cursorId)
        {
            int pageSize = 20; 
            var query = _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Country)
                .AsQueryable();

           
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }
            if (cursorDate.HasValue && cursorId.HasValue)
            {
                query = query.Where(u => u.CreatedAt < cursorDate.Value
                                      || (u.CreatedAt == cursorDate.Value && u.Id < cursorId.Value));
            }
            var entities = await query
                .OrderByDescending(u => u.CreatedAt)
                .ThenByDescending(u => u.Id)
                .Take(pageSize)
                .ToListAsync();

           
            var dtos = entities.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.Role.Role1, 
                CountryName = u.Country?.CountryName ?? "Unknown",
                Verified = u.Verified ?? false,
                CreatedAt = u.CreatedAt
            }).ToList();

            
            var model = new AdminUsersIndexViewModel
            {
                Users = new CursorResult<UserDto>
                {
                    Items = dtos,
                    HasNextPage = dtos.Count == pageSize
                },
                SearchTerm = searchTerm,
                
            };

            
            if (dtos.Any())
            {
                var lastItem = dtos.Last();
                model.Users.NextCursorDate = lastItem.CreatedAt;
                model.Users.NextCursorId = lastItem.Id;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var model = new AdminUserFormViewModel
            {
                Roles = await GetRolesList(),
                Countries = await GetCountriesList()
            };
            return View("UserForm", model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(AdminUserFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var salt = _userService.GenerateSalt();
                var hash = _userService.HashPassword(model.NewPassword, salt);
                var newUser = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    RoleId = model.RoleId,
                    CountryId = model.CountryId,
                    Verified = model.Verified,
                    CreatedAt = DateTime.Now,
                    Salt = salt,
                    PasswordHash = hash
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }

            model.Roles = await GetRolesList();
            model.Countries = await GetCountriesList();
            return View("UserForm", model);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var model = new AdminUserFormViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                CountryId = user.CountryId,
                Verified = user.Verified ?? false,
                Roles = await GetRolesList(),
                Countries = await GetCountriesList()
            };
            return View("UserForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(AdminUserFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(model.Id);
                if (user == null) return NotFound();

                user.Username = model.Username;
                user.Email = model.Email;
                user.RoleId = model.RoleId;
                user.CountryId = model.CountryId;
                user.Verified = model.Verified;

                
                if (!string.IsNullOrEmpty(model.NewPassword))
                {

                    var salt = _userService.GenerateSalt();
                    var hash = _userService.HashPassword(model.NewPassword, salt);
                    user.PasswordHash = hash;
                    user.Salt = salt;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }

            model.Roles = await GetRolesList();
            model.Countries = await GetCountriesList();
            return View("UserForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        // helpers for list Roles&Countries
        public async Task<List<SelectListItem>> GetRolesList()
        {
            return await _context.Roles
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Role1 })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCountriesList()
        {
            return await _context.Countries
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CountryName })
                .ToListAsync();
        }

        // manage events

        [HttpGet]
        public async Task<IActionResult> Events(string? searchTerm, int? status, DateTime? cursorDate, int? cursorId)
        {
            var result = await _eventService.SearchEventsAsync(
                title: searchTerm,
                city: null,
                countryId: null,
                typeId: null,
                minPrice: null,
                maxPrice: null,
                startDateFrom: null,
                startDateTo: null,
                status: status,
                cursorDate: cursorDate,
                cursorId: cursorId,
                pageSize: 20 
            );

            var model = new AdminEventsIndexViewModel
            {
                Events = new CursorResult<EventDto>
                {
                    Items = result.Items,
                    NextCursorDate = result.NextCursorDate,
                    NextCursorId = result.NextCursorId,
                    HasNextPage = result.HasNextPage,
                },
                SearchTerm = searchTerm,
                StatusFilter = status
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Performers(string? searchTerm, int? status, DateTime? cursorDate, int? cursorId)
        {
            var result = await _performerRepo.SearchPerformersAsync(
                searchTerm,
                status,
                cursorDate,
                cursorId,
                pageSize: 20
            );

            var model = new AdminPerformersIndexViewModel
            {
                Performers = result,
                SearchTerm = searchTerm,
                StatusFilter = status
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {

            if (!User.IsInRole("SuperAdmin"))
            {
                return Forbid();
            }
            var success = await _eventService.DeleteEventAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction(nameof(Events));
            }

            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction(nameof(Events));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePerformer(int id)
        {
          
            if (!User.IsInRole("SuperAdmin"))
            {
                return Forbid(); 
            }
            var success = await _performerRepo.DeleteAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = "Performer not found.";
                return RedirectToAction(nameof(Performers));
            }

            TempData["SuccessMessage"] = "Performer deleted successfully.";
            return RedirectToAction(nameof(Performers));
        }
    }
}
