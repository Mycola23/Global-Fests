namespace GlobalFests.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Poster { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TicketPrice { get; set; }
        public string City { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public int Status { get; set; }
        public int TicketAmount { get; set; }
        public string OrganizerName { get; set; } = null!;
    }
}
