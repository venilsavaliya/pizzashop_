namespace DAL.ViewModels;

public class AddModifierGroupViewModel
{ 
    public int? ModifierId { get; set; } 
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public List<int> ModifierItems {get;set;}
}
