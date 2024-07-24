using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbOrgUser
{
    public string Uid { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string Name { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public bool Enable { get; set; }

    public string? OauthProvIder { get; set; }

    public string? LogId { get; set; }

    public virtual ICollection<TbOrgDeptUser> TbOrgDeptUsers { get; set; } = new List<TbOrgDeptUser>();

    public virtual ICollection<TbOrgRole> Rids { get; set; } = new List<TbOrgRole>();
}
