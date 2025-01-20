namespace BumboSolid.Data.Models
{
	public class CLASurchargeEntry
	{
		public int Id { get; set; }
		public int CLAEntryId { get; set; }
		public int Surcharge { get; set; }
		public byte? Weekday { get; set; }
		public TimeOnly? StartTime { get; set; }
		public TimeOnly? EndTime { get; set; }
		public virtual CLAEntry CLAEntry { get; set; } = null!;
	}
}
