using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Modifiersgroup
{
    public int ModifiergroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual ICollection<Modifieritemsmodifiersgroup> Modifieritemsmodifiersgroups { get; } = new List<Modifieritemsmodifiersgroup>();

    public virtual User? ModifyiedbyNavigation { get; set; }
}
