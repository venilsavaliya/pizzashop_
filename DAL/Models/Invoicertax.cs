using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Invoicertax
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public string TaxName { get; set; } = null!;

    public string Taxtype { get; set; } = null!;

    public decimal TaxAmount { get; set; }

    public int TaxId { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Taxis Tax { get; set; } = null!;
}
