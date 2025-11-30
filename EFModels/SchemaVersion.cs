using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GlobalFests.EFModels;

[Index("ScriptHash", Name = "IX_SchemaVersions_ScriptHash")]
public partial class SchemaVersion
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string ScriptName { get; set; } = null!;

    [StringLength(64)]
    public string ScriptHash { get; set; } = null!;

    public DateTimeOffset AppliedAt { get; set; }
}
