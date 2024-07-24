using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbMenu
{
    public string MenuId { get; set; } = null!;

    public string? ParentMenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public string? Icon { get; set; }

    public string? Url { get; set; }

    public bool Enable { get; set; }

    public int Sort { get; set; }

    public virtual ICollection<TbAuth> TbAuths { get; set; } = new List<TbAuth>();
}
