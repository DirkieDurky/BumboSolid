using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Holiday
{
    public string Name { get; set; } = null!;

    public virtual HolidayDay? HolidayDay { get; set; }
}
