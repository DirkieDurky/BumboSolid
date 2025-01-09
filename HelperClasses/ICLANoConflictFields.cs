using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public interface ICLANoConflictFields
    {
        public bool NoConflicts(CLAEntry existingEntry, CLAManageViewModel model, ModelStateDictionary modelState);

    }
}
