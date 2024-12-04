using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class HolidayDay
{
    public string HolidayName { get; set; } = null!;

    public DateOnly Date { get; set; }

    public short Impact { get; set; }

    public virtual Holiday HolidayNameNavigation { get; set; } = null!;
}
