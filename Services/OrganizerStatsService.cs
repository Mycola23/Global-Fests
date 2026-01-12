using GlobalFests.Data;
using GlobalFests.EFModels;
using GlobalFests.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
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
            var paramOrganizerId = new SqlParameter("@OrganizerId", organizerId);

            var result = await _context.Database
                .SqlQueryRaw<GeneralRevenueStats>(
                    "EXEC GetOrganizerGeneralStats @OrganizerId",
                    paramOrganizerId)
                .ToListAsync();

            return result.FirstOrDefault() ?? new GeneralRevenueStats { TotalRevenue = 0, TotalTicketsSold = 0 };
        }

        public async Task<List<OrganizerPanelStats.MonthlySalesData>> GetMonthlySalesDataAsync(int organizerId)
        {
            var paramOrganizerId = new SqlParameter("@OrganizerId", organizerId);

            return await _context.Database
                .SqlQueryRaw<MonthlySalesData>(
                    "EXEC GetOrganizerMonthlySales @OrganizerId",
                    paramOrganizerId)
                .ToListAsync();
        }

        public async Task<List<EventTypeSalesData>> GetEventTypeSalesDataAsync(int organizerId)
        {
            var paramOrganizerId = new SqlParameter("@OrganizerId", organizerId);

            return await _context.Database
                .SqlQueryRaw<EventTypeSalesData>(
                    "EXEC GetOrganizerEventTypeSales @OrganizerId",
                    paramOrganizerId)
                .ToListAsync();
        }


        public async Task<List<EventCountrySalesData>> GetEventCountrySalesDataAsync(int organizerId)
        {
            var paramOrganizerId = new SqlParameter("@OrganizerId", organizerId);

            return await _context.Database
                .SqlQueryRaw<EventCountrySalesData>(
                    "EXEC GetOrganizerCountrySales @OrganizerId",
                    paramOrganizerId)
                .ToListAsync();
        }
    }
}
