using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Index("Role1", Name = "UQ__Roles__DA15413EB6B855A2", IsUnique = true)]
public partial class Role
{
    [Key]
    public int Id { get; set; }

    [Column("Role")]
    [StringLength(500)]
    public string Role1 { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
