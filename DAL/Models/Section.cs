using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Section
{
    public int SectionId { get; set; }

    public string SectionName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual ICollection<Diningtable> Diningtables { get; } = new List<Diningtable>();

    public virtual User? ModifyiedbyNavigation { get; set; }

    public virtual ICollection<Waitingtoken> Waitingtokens { get; } = new List<Waitingtoken>();
}
