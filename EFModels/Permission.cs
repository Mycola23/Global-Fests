using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Index("Permission1", Name = "UQ__Permissi__F5C738EB0B05906F", IsUnique = true)]
public partial class Permission
{
    [Key]
    public int Id { get; set; }

    [Column("Permission")]
    [StringLength(1000)]
    public string Permission1 { get; set; } = null!;

    [ForeignKey("PermissionId")]
    [InverseProperty("Permissions")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
