using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public interface ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState);
        
    }
}
