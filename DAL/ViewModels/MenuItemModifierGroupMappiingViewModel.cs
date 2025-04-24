namespace DAL.ViewModels;

public class MenuItemModifierGroupMappiingViewModel
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public short Rate { get; set; }

    public double TaxPercentage { get; set; }

    public List<ModifierGroupMinMaxMapping> ModifierGroups {get;set;} = new List<ModifierGroupMinMaxMapping>();

}


public class ModifierGroupMinMaxMapping
{
    public int ModifiergroupId { get; set; }

    public string Name { get; set; } = null!;

    public int MinValue { get; set; }

    public int MaxValue { get; set; }

    public List<ModifierItemNamePriceViewModel> ModifierItems { get; set; } = new List<ModifierItemNamePriceViewModel>();
}

public class ModifierItemNamePriceViewModel
{
    public int ModifierId { get; set; }

    public string ModifierName { get; set; } = null!;

    public int? Rate { get; set; }
}