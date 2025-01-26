using BumboSolid.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class CreatePrognosisViewModel
{
    [Required(ErrorMessage = "Dit veld is vereist")]
    public required Week Prognosis;

    [Required(ErrorMessage = "Dit veld is vereist")]
    public List<Weather> WeatherValues = new List<Weather>();

    [Required(ErrorMessage = "Dit veld is vereist")]
    public List<Norm> Norms = new List<Norm>();
}