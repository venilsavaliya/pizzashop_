namespace DAL.ViewModels;

public class OrderAppModifierItemList
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = null!;

    public List<OrderAppModifier> Modifiergroups { get; set; }


}

public class OrderAppModifier
{
    public int ModifiergroupId { get; set; }
    public string Name { get; set; } = null!;
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public List<OrderAppModifierItemsDetail> ModifierItems { get; set; }
}

public class OrderAppModifierItemsDetail
{
    public int ModifierId { get; set; }

    public string ModifierName { get; set; } = null!;

    public int? Rate { get; set; }

}