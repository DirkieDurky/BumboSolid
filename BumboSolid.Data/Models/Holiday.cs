using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Holiday
{
    public string Name { get; set; } = null!;

    public virtual ICollection<HolidayDay?> HolidayDays { get; set; } = new List<HolidayDay?>();
}
