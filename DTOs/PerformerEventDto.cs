namespace GlobalFests.DTOs
{
    public class PerformerEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public string? City { get; set; }
        public string? Poster { get; set; }
    }
}
