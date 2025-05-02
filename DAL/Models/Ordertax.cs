using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Ordertax
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? Taxid { get; set; }

    public string? Taxname { get; set; }

    public string? Taxtype { get; set; }

    public decimal? Taxamount { get; set; }

    public virtual Order? Order { get; set; }
}
