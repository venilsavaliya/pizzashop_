using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class PaymentMode
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
