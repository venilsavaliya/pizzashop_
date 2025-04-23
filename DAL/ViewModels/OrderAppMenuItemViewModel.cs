namespace DAL.ViewModels;

public class OrderAppMenuItemViewModel
{
    public int ItemId { get; set; }
    public string Type { get; set; } = null!;
    public string Image { get; set; }
    public short Rate { get; set; }
    public string ItemName { get; set; } = null!;
    public bool Isfavourite { get; set; }
}
