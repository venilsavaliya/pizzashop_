using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Itemsminmaxmapping
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int MinValue { get; set; }

    public int MaxValue { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual Item? Item { get; set; }
}
