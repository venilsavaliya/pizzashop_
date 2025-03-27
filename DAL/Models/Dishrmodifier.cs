using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Dishrmodifier
{
    public int Id { get; set; }

    public int Dishid { get; set; }

    public int Modifieritemid { get; set; }

    public string? Modifieritemname { get; set; }

    public short? Modifieritemprice { get; set; }

    public int? Quantity { get; set; }

    public virtual Dishritem Dish { get; set; } = null!;

    public virtual Modifieritem Modifieritem { get; set; } = null!;
}
