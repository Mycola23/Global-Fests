namespace GlobalFests.DTOs
{
    public class CreateReviewDto
    {
        public int EventId { get; set; }
        public bool IsLike { get; set; }
        public string? Comment { get; set; }
    }

    
    public class ReviewViewDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsLike { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    
    public class EventRatingSummaryDto
    {
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
    }
}
