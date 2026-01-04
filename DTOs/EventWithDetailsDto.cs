using System.ComponentModel.DataAnnotations.Schema;
using GlobalFests.EFModels;

namespace GlobalFests.DTOs
{
    public class EventWithDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Poster { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TicketPrice { get; set; }
        public int TicketAmount { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string CountryName { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public string OrganizerName { get; set; } = null!;
        public string OrganizerEmail { get; set; } = null!;
        public int Status { get; set; }
        public List<Genre> Genres { get; set; } = new();
        public List<Performer> Performers { get; set; } = new();
    }
}
