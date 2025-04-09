using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class AddEditWaitingTokenViewModel
{
    public int? Tokenid { get; set; } 

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string Mobile { get; set; } = null!;

    public int TotalPerson { get; set; }

    public int Sectionid { get; set; }

}
