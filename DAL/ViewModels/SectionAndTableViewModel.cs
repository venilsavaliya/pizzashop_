using DAL.Models;

namespace DAL.ViewModels;

public class SectionAndTableViewModel
{
     public int? SelectedSection {get;set;}

     public IEnumerable<SectionNameViewModel> Sections {get;set;}

     public IEnumerable<Tablestatus> TableStatus { get; set; } = new List<Tablestatus>();

     public AddTableViewmodel Table { get; set; } = new AddTableViewmodel();

     public AddSectionViewModel Section {get;set;} = new AddSectionViewModel();
}
