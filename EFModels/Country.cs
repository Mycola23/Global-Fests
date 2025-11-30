using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Index("CountryCode", Name = "UQ__Countrie__5D9B0D2C1A35DEA8", IsUnique = true)]
[Index("CountryName", Name = "UQ__Countrie__E056F201C2EA763F", IsUnique = true)]
public partial class Country
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string CountryName { get; set; } = null!;

    [StringLength(5)]
    public string CountryCode { get; set; } = null!;

    [InverseProperty("Country")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    [InverseProperty("Country")]
    public virtual ICollection<Performer> Performers { get; set; } = new List<Performer>();

    [InverseProperty("Country")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
