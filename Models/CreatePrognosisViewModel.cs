using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
	public class CreatePrognosisViewModel
	{
		public required Week Prognosis;
		public List<Weather> WeatherValues = new List<Weather>();
		public List<Norm> Norms = new List<Norm>();
	}
}
