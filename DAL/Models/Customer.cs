using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string Mobile { get; set; } = null!;

    public int TotalVisit { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public int? Totalperson { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual ICollection<Diningtable> Diningtables { get; } = new List<Diningtable>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<Waitingtoken> Waitingtokens { get; } = new List<Waitingtoken>();
}
