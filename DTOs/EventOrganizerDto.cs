using GlobalFests.EFModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GlobalFests.DTOs
{
    public class EventOrganizerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Poster { get; set; }
        //public bool? Approved { get; set; }
        public int Status { get; set; }
        [ValidateNever]
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Genre>? Genres { get; set; } = new List<Genre> { null! };
    }
}
