using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<State> States { get; } = new List<State>();
}
