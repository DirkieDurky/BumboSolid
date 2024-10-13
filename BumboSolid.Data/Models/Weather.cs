using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Weather
{
    public byte Id { get; set; }

    public short Impact { get; set; }

    public virtual ICollection<Factor> Factors { get; set; } = new List<Factor>();
}
