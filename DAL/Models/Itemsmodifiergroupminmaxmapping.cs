using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Itemsmodifiergroupminmaxmapping
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int ModifiergroupId { get; set; }

    public int MinValue { get; set; }

    public int MaxValue { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Modifiersgroup Modifiergroup { get; set; } = null!;
}
