using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class WeatherManageViewModel
{
    [Required(ErrorMessage = "Dit veld is vereist")]
    public short[] Impacts = new short[7];
}