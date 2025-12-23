using GlobalFests.EFModels;

namespace GlobalFests.DTOs
{
    public class PerformerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public bool Approved { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<Genre>? Genres { get; set; } = new List<Genre> { null! };
    }
}
