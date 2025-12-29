using GlobalFests.EFModels;

namespace GlobalFests.DTOs
{
    public class PerformerWithDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Avatar { get; set; }
        public string CountryName { get; set; } = "Unknown";
        public DateTime CreatedAt { get; set; }

        // only for admin
        public int Status { get; set; }
        public string? RejectionReason { get; set; }
        public string CreatorName { get; set; } = "Unknown";
        // ========================================================
        public List<Genre> Genres { get; set; } = new();
        public List<PerformerEventDto> Events { get; set; } = new();
    }
}
