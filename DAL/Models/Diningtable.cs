using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Diningtable
{
    public int TableId { get; set; }

    public int? SectionId { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual User? ModifyiedbyNavigation { get; set; }

    public virtual Section? Section { get; set; }

    public virtual ICollection<Tableorder> Tableorders { get; } = new List<Tableorder>();
}
