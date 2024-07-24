using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbOrgDept
{
    public string DeptId { get; set; } = null!;

    public string DeptName { get; set; } = null!;

    public string? ParentDeptId { get; set; }

    public string RootDeptId { get; set; } = null!;

    public bool Enable { get; set; }

    public int Sort { get; set; }

    public bool Expand { get; set; }

    public string? LogId { get; set; }

    public virtual ICollection<TbOrgDeptUser> TbOrgDeptUsers { get; set; } = new List<TbOrgDeptUser>();
}
