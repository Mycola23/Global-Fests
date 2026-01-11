using GlobalFests.EFModels;

namespace GlobalFests.Repositories
{
    public interface IReviewRepository
    {
        Task AddOrUpdateAsync(Review review);
        Task<List<Review>> GetByEventIdAsync(int eventId);
        Task<(int likes, int dislikes)> GetEventStatsAsync(int eventId);
        
    }
}
