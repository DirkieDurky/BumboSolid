using BumboSolid.Data.Models;

namespace BumboSolid.Web.Models
{
    public class PrognosesViewModel
    {
        public List<Prognosis> Prognoses { get; set; }

        public int Id { get; set; }

        public List<PrognosisFunction> PrognosisFunctions { get; set; } = new List<PrognosisFunction>();
    }
}
