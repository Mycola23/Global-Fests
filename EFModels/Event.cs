using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Table("Events")]
public partial class Event
{
    [Key]
    public int Id { get; set; }

    public int OrganizerId { get; set; }
    [Display(Name = "Event Type")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select an Event Type.")]
    public int TypeId { get; set; }

    [Display(Name = "Country")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Country.")]
    public int CountryId { get; set; }

    [StringLength(2000)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal Latitude { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal Longitude { get; set; }

    [StringLength(1000)]
    public string? Address { get; set; }

    [StringLength(1000)]
    public string? City { get; set; }

    public string? Poster { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    [Display(Name = "Ticket Price")]
    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal? TicketPrice { get; set; }

    [Display(Name = "Ticket Quantity")]
    [Range(1, int.MaxValue, ErrorMessage = "Ticket quantity must be at least 1.")]
    public int TicketAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ValidateNever]
    public int Status { get; set; }
    [ValidateNever]
    public string? RejectionReason { get; set; }

    [ValidateNever]
    public virtual Country Country { get; set; } = null!;
    [ValidateNever]
    public virtual User Organizer { get; set; } = null!;
    [ValidateNever]
    public virtual EventType Type { get; set; } = null!;

    [InverseProperty("Event")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    [InverseProperty("Events")]
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    [InverseProperty("Events")]
    public virtual ICollection<Performer> Performers { get; set; } = new List<Performer>();

    [InverseProperty("Event")]
    public virtual ICollection<WishList> WishList { get; set; } = new List<WishList>();
}