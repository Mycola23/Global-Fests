using GlobalFests.Data;
using GlobalFests.Helpers;
using GlobalFests.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Controllers
{
    public class AdminController : Controller
    {
        private readonly GlobalFestsContext _context;

        public AdminController(GlobalFestsContext context)
        {
            _context = context;
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
    
}
}
