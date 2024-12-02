using BumboSolid.Data.Models;
using System;
using System.Collections.Generic;

namespace BumboSolid.Models;

public class ShiftCreateViewModel
{
	public required List<User> Employees { get; set; }

	public required Shift Shift { get; set; }

	public Int16 Year { get; set; }

	public Int16 Week { get; set; }
}
