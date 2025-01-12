using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.HelperClasses
{
    public interface ICLAEntryConverter
    {
        void EnsureAgeRange(CLAEntry entry, CLAManageViewModel model);
        public CLAEntry ModelToEntry(CLAManageViewModel model, CLAEntry entry);
    }
}
