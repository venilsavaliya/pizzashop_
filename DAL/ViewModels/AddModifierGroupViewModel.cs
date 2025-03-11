namespace DAL.ViewModels;

public class AddModifierGroupViewModel
{ 

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public List<int> ModifieritemsId {get;set;}
}
