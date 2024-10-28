﻿using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class EditPrognosisFactorsViewModel
	{
		public Prognosis Prognosis { get; set; }
		public List<Weather> WeatherValues { get; set; }

		// These properties hold the user input
		public int[] VisitorEstimates { get; set; }
		public int[] Holidays { get; set; }
		public int[] WeatherIds { get; set; }
		public int[] Others { get; set; }
		public string[] Descriptions { get; set; }
	}

}
