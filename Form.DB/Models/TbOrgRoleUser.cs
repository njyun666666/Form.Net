using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbOrgRoleUser
{
    public string Uid { get; set; } = null!;

    public string Rid { get; set; } = null!;

    public string RootDeptId { get; set; } = null!;

    public virtual TbOrgRole RidNavigation { get; set; } = null!;

    public virtual TbOrgDept RootDept { get; set; } = null!;

    public virtual TbOrgUser UidNavigation { get; set; } = null!;
}
