using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Index("Type", Name = "UQ__EventTyp__F9B8A48B84B0415B", IsUnique = true)]
public partial class EventType
{
    [Key]
    public int Id { get; set; }

    [StringLength(2000)]
    public string Type { get; set; } = null!;

    [InverseProperty("Type")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
