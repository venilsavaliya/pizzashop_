namespace DAL.ViewModels;

public class OrderDetailKOTViewModel
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string SectionName { get; set; }

    public List<string> TableNames { get; set; } = new List<string>();

    public List<OrderItemKOTViewModel> OrderItems { get; set; } = new List<OrderItemKOTViewModel>();

    public string Instruction { get; set; } = null!;
}

public class OrderItemKOTViewModel
{
    public string ItemName { get; set; } = null!;

    public List<OrderModifierViewModel> ModifierList { get; set; }

    public int? PendingQuantity { get; set; }

    public int? ReadyQuantity { get; set; }

    public short? ItemPrice { get; set; }

    public string? Instruction { get; set; }

}

public class OrderDishKOTViewModel
{
    public int DishId { get; set; }
    public string ItemName { get; set; } = null!;
    public List<OrderModifierViewModel> ModifierList { get; set; }

    public int? PendingQuantity { get; set; }

    public int? ReadyQuantity { get; set; }

    public int TotalQuantity {get;set;}
}
