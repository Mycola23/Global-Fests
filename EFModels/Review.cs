using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobalFests.EFModels
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int EventId { get; set; }

        public bool IsLike { get; set; } // true for like \ false - dis
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; } = null!;
    }
}