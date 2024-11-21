using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class CLABreakEntry
{
    public int CaoentryId { get; set; }

    public int WorkDuration { get; set; }

    public int? MinBreakDuration { get; set; }

    public virtual CLAEntry Caoentry { get; set; } = null!;
}
