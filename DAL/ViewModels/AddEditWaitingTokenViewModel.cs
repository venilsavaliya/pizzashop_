using System;
using System.Collections.Generic;
using DAL.ViewModels;

namespace DAL.ViewModels;

public partial class AddEditWaitingTokenViewModel  
{
    public int? Tokenid { get; set; } 
    public int Sectionid { get; set; }
    public AddCustomerViewModel Customer {get;set;} = new AddCustomerViewModel();
    public List<SectionNameViewModel> Sections = new List<SectionNameViewModel>();

}
