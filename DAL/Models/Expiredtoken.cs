using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Expiredtoken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;
}
