using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Dishritem
{
    public int Dishid { get; set; }

    public int Itemid { get; set; }

    public string? Itemname { get; set; }

    public short? Itemprice { get; set; }

    public int Orderid { get; set; }

    public int? Quantity { get; set; }

    public int? Pendingquantity { get; set; }

    public int? Inprogressquantity { get; set; }

    public int? Readyquantity { get; set; }

    public virtual ICollection<Dishrmodifier> Dishrmodifiers { get; } = new List<Dishrmodifier>();

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
