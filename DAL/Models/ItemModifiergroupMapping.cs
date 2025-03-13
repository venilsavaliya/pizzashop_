using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ItemModifiergroupMapping
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int? ModifierGroupId { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Modifiersgroup? ModifierGroup { get; set; }
}
