using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbOrgDeptUser
{
    public string DeptId { get; set; } = null!;

    public string Uid { get; set; } = null!;

    public bool Enable { get; set; }

    public virtual TbOrgDept Dept { get; set; } = null!;

    public virtual TbOrgUser UidNavigation { get; set; } = null!;
}
