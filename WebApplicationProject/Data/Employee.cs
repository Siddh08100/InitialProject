using System;
using System.Collections.Generic;

namespace WebApplicationProject.Data;

public partial class Employee
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public int? Deptid { get; set; }

    public int? Age { get; set; }

    public string Email { get; set; } = null!;

    public string Education { get; set; } = null!;

    public string? Company { get; set; }

    public short? Experience { get; set; }

    public decimal? Package { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Gender { get; set; }

    public virtual Department? Dept { get; set; }
}
