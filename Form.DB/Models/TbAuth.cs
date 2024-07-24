using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbAuth
{
    public int Id { get; set; }

    public string MenuId { get; set; } = null!;

    public int AuthType { get; set; }

    public string TargetId { get; set; } = null!;

    public sbyte IncludeChildren { get; set; }

    public virtual TbMenu Menu { get; set; } = null!;
}
