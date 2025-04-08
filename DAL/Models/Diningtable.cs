using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Diningtable
{
    public int TableId { get; set; }

    public int? SectionId { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public int Status { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? AssignTime { get; set; }

    public int? CurrentOrderId { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual Order? CurrentOrder { get; set; }

    public virtual User? ModifyiedbyNavigation { get; set; }

    public virtual Section? Section { get; set; }

    public virtual Tablestatus StatusNavigation { get; set; } = null!;

    public virtual ICollection<Tableorder> Tableorders { get; } = new List<Tableorder>();
}
