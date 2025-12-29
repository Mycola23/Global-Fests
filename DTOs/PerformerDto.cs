using GlobalFests.EFModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GlobalFests.DTOs
{
    public class PerformerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public int Status { get; set; }
        [ValidateNever]
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Genre>? Genres { get; set; } = new List<Genre> { null! };
    }
}
