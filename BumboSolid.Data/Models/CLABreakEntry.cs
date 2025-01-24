namespace BumboSolid.Data.Models;

public partial class CLABreakEntry
{
    public int CLAEntryId { get; set; }

    public int WorkDuration { get; set; }

    public int? MinBreakDuration { get; set; }

    public virtual CLAEntry CLAEntry { get; set; } = null!;
}