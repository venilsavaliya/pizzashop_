namespace DAL.ViewModels;

public class AddSectionViewModel
{   
    public int? Sectionid {get;set;}
    public string SectionName { get; set; } = null!;

    public string? Description { get; set; }
}
