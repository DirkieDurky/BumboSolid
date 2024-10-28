using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class EditFactorsViewModel
	{
		public Prognosis Prognosis;
		public Dictionary<byte, int>? VisitorEstimatePerDay = new Dictionary<byte, int>();
		public List<Weather> WeatherValues = new List<Weather>();
		public List<Norm> Norms = new List<Norm>();
	}
}
