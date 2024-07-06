using System;
using System.Collections.Generic;

namespace FormDB.Models;

public partial class TbRefreshToken
{
    public string RefreshToken { get; set; } = null!;

    public DateTime ExpireTime { get; set; }

    public string Uid { get; set; } = null!;
}
