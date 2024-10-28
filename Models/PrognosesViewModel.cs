using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
    public class PrognosesViewModel
    {
        public required List<Prognosis> Prognoses { get; set; }

        public int? Id { get; set; }

        public List<PrognosisFunction> PrognosisFunctions { get; set; } = new List<PrognosisFunction>();
    }
}
