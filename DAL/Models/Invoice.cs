using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int OrderId { get; set; }

    public DateTime? Paidon { get; set; }

    public virtual ICollection<Invoicertax> Invoicertaxes { get; } = new List<Invoicertax>();

    public virtual Order Order { get; set; } = null!;
}
