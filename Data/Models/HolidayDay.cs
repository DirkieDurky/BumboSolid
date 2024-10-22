using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class HolidayDay
{
    public string HolidayName { get; set; } = null!;

    public DateOnly Date { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Impact moet een waarde tussen 1 en 2147483647 hebben.")]
    public short Impact { get; set; }

    public virtual Holiday HolidayNameNavigation { get; set; } = null!;
}
