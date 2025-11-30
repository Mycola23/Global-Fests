namespace GlobalFests.DTOs
{
    public class EventsByTypeDto
    {
        public string EventType { get; set; } = null!;
        public int EventCount { get; set; }
        public int TotalTickets { get; set; }
        public decimal? AveragePrice { get; set; }
    }
}
