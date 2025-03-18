namespace DAL.ViewModels;

public class AddModifierItemViewModel
{   
    public int? ModifierId { get; set; } 
    public int ModifierGroupid {get;set;}

    public string ModifierName { get; set; } = null!;

    public int? Rate { get; set; }

    public string? Unit { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }
}
