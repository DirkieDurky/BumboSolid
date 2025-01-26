using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public partial class Norm
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Dit veld is vereist")]
    [RegularExpression("^[a-zA-Z0-9\\s':/]*$", ErrorMessage = "Alleen alfanumerieke tekens en spaties zijn toegestaan.")]
    public string Activity { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
    public string Department { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
    [Range(1, int.MaxValue, ErrorMessage = "Duur moet een waarde tussen 1 en 2147483647 hebben.")]
    public int Duration { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    [Range(1, byte.MaxValue, ErrorMessage = "Gemiddeld dagelijks aantal uitvoeringen moet een waarde hebben tussen 1 en 255.")]
    public byte AvgDailyPerformances { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public bool PerVisitor { get; set; }

    public virtual Department? DepartmentNavigation { get; set; } = null!;
}