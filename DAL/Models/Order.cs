using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int CustomerId { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string PaymentMode { get; set; } = null!;

    public short? Rating { get; set; }

    public DateTime? CompletedTime { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool Isdeleted { get; set; }

    public string? Instruction { get; set; }

    public decimal TotalAmount { get; set; }

    public int? TotalPerson { get; set; }

    public DateTime? Placeon { get; set; }

    public int? Ordertype { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Diningtable> Diningtables { get; } = new List<Diningtable>();

    public virtual ICollection<Dishritem> Dishritems { get; } = new List<Dishritem>();

    public virtual Invoice? Invoice { get; set; }

    public virtual User? ModifyiedbyNavigation { get; set; }

    public virtual ICollection<Ordertax> Ordertaxes { get; } = new List<Ordertax>();

    public virtual ICollection<Tableorder> Tableorders { get; } = new List<Tableorder>();
}
