namespace BumboSolid.Data.Models;

public partial class Holiday
{
    public string Name { get; set; } = null!;

    public virtual List<HolidayDay> HolidayDays { get; set; } = new List<HolidayDay>();
}
