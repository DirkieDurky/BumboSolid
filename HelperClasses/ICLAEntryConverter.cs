using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.HelperClasses
{
    public interface ICLAEntryConverter
    {
        void EnsureAgeRange(CLAEntry entry, CLAManageViewModel model);
        public CLAEntry ModelToEntry(CLAManageViewModel model, CLAEntry entry);

        public CLABreakEntry ModelToBreakEntry(CLAManageViewModel model, int claEntryId, CLABreakEntry breakEntry);

        public CLAManageViewModel EntryToModel(CLAEntry entry, CLABreakEntry? breakEntry);
    }
}
