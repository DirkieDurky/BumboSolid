﻿namespace BumboSolid.Models
{
    public class CLACardViewModel
    {
        public int? AgeStart { get; set; }
        public int? AgeEnd { get; set; }
        public List<string> Rules { get; set; } = new List<string>();
    }
}
