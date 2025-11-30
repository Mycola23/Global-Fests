namespace GlobalFests.DTOs
{
    public class EventsByCountryDto
    {
        public string CountryName { get; set; } = null!;
        public int EventCount { get; set; }
        public decimal? AverageTicketPrice { get; set; }
        public decimal? MaxTicketPrice { get; set; }
        public decimal? MinTicketPrice { get; set; }
    }
}
