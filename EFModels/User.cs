using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Index("Email", Name = "UQ__Users__A9D10534B77784BC", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(2000)]
    public string Username { get; set; } = null!;

    [StringLength(2000)]
    public string Email { get; set; } = null!;

    [StringLength(4000)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(4000)]
    public string Salt { get; set; } = null!;

    public int RoleId { get; set; }

    public int? CountryId { get; set; }

    public bool? Verified { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Users")]
    public virtual Country? Country { get; set; }

    [InverseProperty("Organizer")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    [InverseProperty("User")]
    public virtual ICollection<WishList> WishList { get; set; } = new List<WishList>();
}
