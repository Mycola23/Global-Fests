using GlobalFests.Data;
using GlobalFests.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly GlobalFestsContext _context;

        public ReviewRepository(GlobalFestsContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateAsync(Review review)
        {

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.EventId == review.EventId && r.UserId == review.UserId);

            if (existingReview != null)
            {

                existingReview.IsLike = review.IsLike;
                existingReview.Comment = review.Comment;
                existingReview.CreatedAt = DateTime.Now;
                _context.Reviews.Update(existingReview);
            }
            else
            {

                await _context.Reviews.AddAsync(review);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Review>> GetByEventIdAsync(int eventId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<(int likes, int dislikes)> GetEventStatsAsync(int eventId)
        {
            var likes = await _context.Reviews.CountAsync(r => r.EventId == eventId && r.IsLike);
            var dislikes = await _context.Reviews.CountAsync(r => r.EventId == eventId && !r.IsLike);
            return (likes, dislikes);
        }
    }
}