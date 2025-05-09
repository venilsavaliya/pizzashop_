namespace DAL.ViewModels;

public class AddModifierGroupViewModel
{ 
    public int? ModifierId { get; set; } 
    
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public List<int> ModifierItems {get;set;} = new List<int>();
}
