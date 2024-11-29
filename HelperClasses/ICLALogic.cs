using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public interface ICLALogic
    {

        public CLAEntry ViewModelToEntry(CLAManageViewModel model);

        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState);

    }
}
