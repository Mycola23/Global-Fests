using GlobalFests.DTOs;

namespace GlobalFests.Services
{
    public interface IReviewService
    {
        Task AddReviewAsync(int userId, CreateReviewDto dto);
        Task<List<ReviewViewDto>> GetEventReviewsAsync(int eventId);
        Task<EventRatingSummaryDto> GetRatingAsync(int eventId);
    }
}
