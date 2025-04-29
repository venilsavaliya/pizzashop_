using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Taxis
{
    public int TaxId { get; set; }

    public string TaxName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public decimal TaxAmount { get; set; }

    public bool? Isenable { get; set; }

    public bool Isdefault { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool Isdeleted { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual ICollection<Invoicertax> Invoicertaxes { get; } = new List<Invoicertax>();

    public virtual User? ModifyiedbyNavigation { get; set; }

    public virtual ICollection<Ordertax> Ordertaxes { get; } = new List<Ordertax>();
}
