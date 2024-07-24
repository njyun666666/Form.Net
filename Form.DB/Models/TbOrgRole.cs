using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbOrgRole
{
    public string Rid { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public virtual ICollection<TbOrgUser> Uids { get; set; } = new List<TbOrgUser>();
}
