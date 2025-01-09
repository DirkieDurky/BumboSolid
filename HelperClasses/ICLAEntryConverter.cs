using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.HelperClasses
{
    public interface ICLAEntryConverter
    {
        public CLAEntry ModelToEntry(CLAManageViewModel model);

        CLAManageViewModel EntryToModel(CLAEntry entry, CLABreakEntry? breakEntry);
        decimal CalculateAndRound(int? field, decimal factor);
    }
}
