using BumboSolid.Data.Models;
using System;
using System.Collections.Generic;

namespace BumboSolid.Models;

public class ShiftCreateViewModel
{
	public required Shift Shift { get; set; }

	public required Dictionary<Int32, String>? Employees { get; set; }

	public required Week? Week { get; set; }
}
