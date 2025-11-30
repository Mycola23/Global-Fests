using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

public partial class Ticket
{
    [Key]
    public int Id { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("Tickets")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Tickets")]
    public virtual User User { get; set; } = null!;
}
