using GlobalFests.EFModels;

namespace GlobalFests.DTOs
{
    public class EventOrganizerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Poster { get; set; }
        public bool? Approved { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<Genre>? Genres { get; set; } = new List<Genre> { null! };
    }
}
