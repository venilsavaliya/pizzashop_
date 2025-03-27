using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Tableorder
{
    public int Id { get; set; }

    public int? TableId { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Diningtable? Table { get; set; }
}
