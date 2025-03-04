namespace DAL.ViewModels;

public class ItemViewModel
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public short Rate { get; set; }

    public short Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public bool Isavailable { get; set; }

    public string? Image { get; set; }

}
