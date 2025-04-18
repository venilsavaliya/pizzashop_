namespace DAL.ViewModels;

public class ItemViewModel
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public short Rate { get; set; }

    public short Quantity { get; set; }

    public int Unit { get; set; } 

    public bool Isavailable { get; set; }

    public string? Image { get; set; } 

    public bool DefaultTax { get; set; }

    public double TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public string? Description { get; set; }


}
