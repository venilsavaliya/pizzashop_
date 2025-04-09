using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Waitingtoken
{
    public int Tokenid { get; set; }

    public int Customerid { get; set; }

    public int Sectionid { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime? Completiontime { get; set; }

    public DateTime? Updatetime { get; set; }

    public Guid Createdby { get; set; }

    public Guid Modifiedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual User ModifiedbyNavigation { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;
}
