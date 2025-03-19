namespace DAL.ViewModels;

public class SectionAndTableViewModel
{
     public int? SelectedSection {get;set;}

     public IEnumerable<SectionNameViewModel> Sections {get;set;}

     public AddTableViewmodel Table { get; set; } = new AddTableViewmodel();

     public AddSectionViewModel Section {get;set;} = new AddSectionViewModel();
}
