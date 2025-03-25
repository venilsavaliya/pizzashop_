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

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual User? ModifyiedbyNavigation { get; set; }
}
