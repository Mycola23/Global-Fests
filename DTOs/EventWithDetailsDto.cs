namespace GlobalFests.DTOs
{
    public class EventWithDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TicketPrice { get; set; }
        public int TicketAmount { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string CountryName { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public string OrganizerName { get; set; } = null!;
        public string OrganizerEmail { get; set; } = null!;
        public List<string> Genres { get; set; } = new();
        public List<string> Performers { get; set; } = new();
    }
}
