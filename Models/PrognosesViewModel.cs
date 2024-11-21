using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
    public class PrognosesViewModel
    {
        public required List<Week> Prognoses { get; set; }

        public int? Id { get; set; }

        public List<PrognosisDepartment> PrognosisDepartments { get; set; } = new List<PrognosisDepartment>();
    }
}
