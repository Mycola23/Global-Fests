using GlobalFests.DTOs;
using GlobalFests.EFModels;
using GlobalFests.Repositories;

namespace GlobalFests.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;

        public ReviewService(IReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task AddReviewAsync(int userId, CreateReviewDto dto)
        {
            var review = new Review
            {
                UserId = userId,
                EventId = dto.EventId,
                IsLike = dto.IsLike,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now
            };

            await _reviewRepo.AddOrUpdateAsync(review);
        }

        public async Task<List<ReviewViewDto>> GetEventReviewsAsync(int eventId)
        {
            var reviews = await _reviewRepo.GetByEventIdAsync(eventId);

            return reviews.Select(r => new ReviewViewDto
            {
                Id = r.Id,
                Username = r.User.Username, 
                IsLike = r.IsLike,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<EventRatingSummaryDto> GetRatingAsync(int eventId)
        {
            var stats = await _reviewRepo.GetEventStatsAsync(eventId);
            return new EventRatingSummaryDto
            {
                LikesCount = stats.likes,
                DislikesCount = stats.dislikes
            };
        }
    }
}
