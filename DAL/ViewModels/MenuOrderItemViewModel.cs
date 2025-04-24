namespace DAL.ViewModels;

public class MenuOrderItemViewModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = null!;

    public short Rate { get; set; }

    public short Quantity { get; set; }

    public double TaxPercentage { get; set; }

    public List<ModifierItemNamePriceViewModel> ModifierItems { get; set; }

    public string ItemComment {get;set;} = "";

}
