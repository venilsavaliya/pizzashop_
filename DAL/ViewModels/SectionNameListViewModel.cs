namespace DAL.ViewModels;

public class SectionNameListViewModel
{ 
    public IEnumerable<SectionNameViewModel>? Sections {get;set;}

    public int? SelectedSection {get;set;}
}
