using System;
using System.Collections.Generic;
using DAL.ViewModels;

namespace DAL.Models;

public partial class AddEditWaitingTokenViewModel  
{
    public int? Tokenid { get; set; } 
    public int Sectionid { get; set; }
    public AddCustomerViewModel Customer {get;set;}

}
