using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobalFests.EFModels;

[Table("WishList")]
public partial class WishList
{
    [Key]
    [Column(Order = 0)]
    public int UserId { get; set; }

    [Key]
    [Column(Order = 1)]
    public int EventId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;
}