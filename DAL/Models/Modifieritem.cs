using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Modifieritem
{
    public int ModifierId { get; set; }

    public string ModifierName { get; set; } = null!;

    public int? Rate { get; set; }

    public string? Unit { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool? Isdeleted { get; set; }

    public string? Photo { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual ICollection<Dishrmodifier> Dishrmodifiers { get; } = new List<Dishrmodifier>();

    public virtual ICollection<Itemsrmodifiersgroup> Itemsrmodifiersgroups { get; } = new List<Itemsrmodifiersgroup>();

    public virtual ICollection<Modifieritemsmodifiersgroup> Modifieritemsmodifiersgroups { get; } = new List<Modifieritemsmodifiersgroup>();

    public virtual User? ModifyiedbyNavigation { get; set; }
}
