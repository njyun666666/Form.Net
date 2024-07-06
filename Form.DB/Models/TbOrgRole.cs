using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbOrgRole
{
    public string Rid { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public virtual ICollection<TbOrgRoleUser> TbOrgRoleUsers { get; set; } = new List<TbOrgRoleUser>();

    public virtual ICollection<TbMenu> Menus { get; set; } = new List<TbMenu>();
}
