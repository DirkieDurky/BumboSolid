using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class FillRequest
{
    public int Id { get; set; }

    public int ShiftId { get; set; }

    public int? SubstituteEmployeeId { get; set; }

    public string? AbsentDescription { get; set; }

    public byte Accepted { get; set; }

    public virtual Shift Shift { get; set; } = null!;

    public virtual Employee? SubstituteEmployee { get; set; }
}
