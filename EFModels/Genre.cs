using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Index("Genre1", Name = "UQ__Genres__F1410CF324E057BD", IsUnique = true)]
public partial class Genre
{
    [Key]
    public int Id { get; set; }

    [Column("Genre")]
    [StringLength(2000)]
    public string Genre1 { get; set; } = null!;

    [ForeignKey("GenreId")]
    [InverseProperty("Genres")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Performer> Performers { get; set; } = new List<Performer>();
}
