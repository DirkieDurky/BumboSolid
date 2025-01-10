using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.HelperClasses
{
    public interface ICLAEntryConverter
    {
        public CLAEntry ModelToEntry(CLAManageViewModel model);
    }
}
