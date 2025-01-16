namespace BumboSolid.Data.Models;

public partial class PrognosisDepartment
{
    public int PrognosisId { get; set; }

    public string Department { get; set; } = null!;

    public byte Weekday { get; set; }

    public short WorkHours { get; set; }

    public virtual Department DepartmentNavigation { get; set; } = null!;

    public virtual PrognosisDay PrognosisDay { get; set; } = null!;
}
