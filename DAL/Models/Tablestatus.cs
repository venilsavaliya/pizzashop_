using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Tablestatus
{
    public int Id { get; set; }

    public string Statusname { get; set; } = null!;

    public virtual ICollection<Diningtable> Diningtables { get; } = new List<Diningtable>();
}
