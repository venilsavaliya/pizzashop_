namespace DAL.ViewModels;

public class MenuOrderItemViewModel
{
    public int ItemId { get; set; }

    public int Index {get;set;}

    public int DishId {get;set;}
    
    public string ItemName { get; set; } = null!;

    public short Rate { get; set; }

    public int Quantity { get; set; }

    public double TaxPercentage { get; set; }

    public List<ModifierItemNamePriceViewModel> ModifierItems { get; set; }

    public string ItemComment {get;set;} = "";

}
