using GlobalFests.Data;
using GlobalFests.EFModels;
using GlobalFests.ViewModels;
using Microsoft.EntityFrameworkCore;
using static GlobalFests.EFModels.OrganizerPanelStats;

namespace GlobalFests.Services
{
    public interface IOrganizerStatsService
    {
        Task<OrganizerPanelStats.GeneralRevenueStats> GetTotalTickets_RevenueAsync(int organizerId);
        Task<List<OrganizerPanelStats.MonthlySalesData>> GetMonthlySalesDataAsync(int organizerId);
        Task<List<EventTypeSalesData>> GetEventTypeSalesDataAsync(int organizerId);

        Task<List<EventCountrySalesData>> GetEventCountrySalesDataAsync(int organizerId);
        Task<OrganizerPanelStats> GetOrganizerAllStats(int organizerId);

        // think about cashing some of data here 
    }

    public class OrganizerStatsService : IOrganizerStatsService
    {
        private readonly GlobalFestsContext _context;
        public OrganizerStatsService(GlobalFestsContext dbContext) {
            _context = dbContext;
        }

        public async Task<OrganizerPanelStats> GetOrganizerAllStats(int organizerId)
        {
            var stats = new OrganizerPanelStats();
            stats.GeneralStats = await GetTotalTickets_RevenueAsync(organizerId);
            stats.MonthlySales = await GetMonthlySalesDataAsync(organizerId);
            stats.EventTypeSales = await GetEventTypeSalesDataAsync(organizerId);
            stats.EventCountrySales = await GetEventCountrySalesDataAsync(organizerId);

            //^ think about this 
            //viewModel.TopPerformers = await GetTopPerformersSalesAsync(organizerId, 5); 
            //viewModel.RecentEvents = await GetRecentEventsStatsAsync(organizerId, 10); 

           

            return stats;
        }
        public async Task<OrganizerPanelStats.GeneralRevenueStats> GetTotalTickets_RevenueAsync(int organizerId)
        {
            var statistics = await _context.Users
            .Where(u => u.Id == organizerId)
            .SelectMany(u => u.Events) 
            .SelectMany(e => e.Tickets) 
            .GroupBy(t => t.Event.Organizer.Username) 
            .Select(g => new
            {
                OrganizerName = g.Key,
                TotalTicketsSold = g.Count(),
                TotalRevenue = g.Sum(t => t.Price)
            })
            .FirstOrDefaultAsync();

            if (statistics == null)
            {
                return new OrganizerPanelStats.GeneralRevenueStats
                {
                    TotalTicketsSold = 0,
                    TotalRevenue = 0M,
                };
            }

            return new OrganizerPanelStats.GeneralRevenueStats
            {
                TotalTicketsSold = statistics.TotalTicketsSold,
                TotalRevenue = statistics.TotalRevenue,
            };
        }

        public async Task<List<OrganizerPanelStats.MonthlySalesData>> GetMonthlySalesDataAsync(int organizerId)
        {
            return await _context.Tickets
                .Where(t => t.Event.OrganizerId == organizerId)
                .Where(t => t.CreatedAt.HasValue) 
                .GroupBy(t => new { Year = t.CreatedAt.Value.Year, Month = t.CreatedAt.Value.Month }) 
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new OrganizerPanelStats.MonthlySalesData 
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    TicketsSold = g.Count(),
                    Revenue = g.Sum(t => t.Price)
                })
                .ToListAsync();
        }

        public async Task<List<EventTypeSalesData>> GetEventTypeSalesDataAsync(int organizerId)
        {
            FormattableString sql = $@"
            SELECT 
                ET.Type AS EventType,
                COUNT(T.Id) AS TicketsSold,
                ISNULL(SUM(T.Price), 0) AS Revenue
            FROM Events E
            JOIN EventTypes ET ON E.TypeId = ET.Id
            JOIN Tickets T ON E.Id = T.EventId
            WHERE E.OrganizerId = {organizerId}
            GROUP BY ET.Type
            ORDER BY Revenue DESC";

            return await _context.Database
                .SqlQuery<EventTypeSalesData>(sql)
                .ToListAsync();
        }


        public async Task<List<EventCountrySalesData>> GetEventCountrySalesDataAsync(int organizerId)
        {
            FormattableString sql = $@"
            SELECT
                C.CountryName,
                COUNT(T.Id) AS TicketsSold,
                ISNULL(SUM(T.Price), 0) AS Revenue
            FROM Users U_Organizer
            JOIN Events E ON U_Organizer.Id = E.OrganizerId
            JOIN Tickets T ON E.Id = T.EventId
            JOIN Users U_Buyer ON T.UserId = U_Buyer.Id
            JOIN Countries C ON U_Buyer.CountryId = C.Id
            WHERE U_Organizer.Id = {organizerId}
            GROUP BY C.CountryName
            ORDER BY TicketsSold DESC";

            return await _context.Database
            .SqlQuery<EventCountrySalesData>(sql)
            .ToListAsync();
        }
    }
}
