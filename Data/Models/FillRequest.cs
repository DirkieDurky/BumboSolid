namespace BumboSolid.Data.Models;

public partial class FillRequest
{
    public int Id { get; set; }

    public int ShiftId { get; set; }

    public int? SubstituteEmployeeId { get; set; }

    public string? AbsentDescription { get; set; }

    public virtual Shift Shift { get; set; } = null!;

    public virtual User? SubstituteEmployee { get; set; }
}
