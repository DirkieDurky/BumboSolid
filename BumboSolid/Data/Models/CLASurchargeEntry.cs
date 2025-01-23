namespace BumboSolid.Data.Models;

public partial class CLASurchargeEntry
{
	public int Id { get; set; }
	public int Surcharge { get; set; }
	public byte? Weekday { get; set; }
	public TimeOnly? StartTime { get; set; }
	public TimeOnly? EndTime { get; set; }
}
