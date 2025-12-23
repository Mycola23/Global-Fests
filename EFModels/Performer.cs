using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

public partial class Performer
{
    [Key]
    public int Id { get; set; }

    [StringLength(2000)]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? CountryId { get; set; }

    [StringLength(4000)]
    public string? Avatar { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Performers")]
    public virtual Country? Country { get; set; }

    [ForeignKey("PerformerId")]
    [InverseProperty("Performers")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public bool Approved { get; set; }

    public int? CreatedBy { get; set; }

   
    [ForeignKey("CreatedBy")]
    [ValidateNever]
    public virtual User? Creator { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
