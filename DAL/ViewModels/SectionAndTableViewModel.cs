namespace DAL.ViewModels;

public class SectionAndTableViewModel
{
     public int? SelectedSection {get;set;}

     public IEnumerable<SectionNameViewModel> Sections {get;set;}
}
