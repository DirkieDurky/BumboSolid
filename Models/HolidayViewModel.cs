﻿using System.ComponentModel.DataAnnotations;
using BumboSolid.Data.Models;

namespace BumboSolid.Web.Models
{
	public class HolidayViewModel
	{
    
        [StringLength(25)]
        [RegularExpression("^[a-zA-Z0-9\\s]*$", ErrorMessage = "Alleen alfanumerieke tekens en spaties zijn toegestaan.")]
        public String Name { get; set; }

        [DataType(DataType.Date)]
        public DateOnly FirstDay { get; set; }

        [DataType(DataType.Date)]
        public DateOnly LastDay { get; set; }
    }
}
