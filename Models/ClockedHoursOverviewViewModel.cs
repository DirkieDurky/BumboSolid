using System.Collections.Generic;
using BumboSolid.Data.Models;

namespace BumboSolid.Models
{
    public class ClockedHoursOverviewViewModel
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int WeekId { get; set; }
        public List<ClockedHours> ClockedHours { get; set; }
        public Dictionary<byte, string> WeekdayDictionary { get; set; }
    }
}
