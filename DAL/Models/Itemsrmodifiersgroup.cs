using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Itemsrmodifiersgroup
{
    public int Id { get; set; }

    public int? ModifiergroupId { get; set; }

    public int? ItemId { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Modifieritem? Modifiergroup { get; set; }
}
