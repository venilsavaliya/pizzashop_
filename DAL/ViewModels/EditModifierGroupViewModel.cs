namespace DAL.ViewModels;

public class EditModifierGroupViewModel
{
    public int ModifierId { get; set; } 

    public string ModifierName { get; set; } = null!;

    public string? Description { get; set; }

    public List<int> ModifierItems {get;set;}
}
 