using GlobalFests.EFModels;

namespace GlobalFests.DTOs
{
    public class EventWorldMapDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Poster { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string City { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public int Status { get; set; }
        public int TicketAmount { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
