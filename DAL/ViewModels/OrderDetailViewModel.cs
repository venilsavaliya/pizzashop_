namespace DAL.ViewModels;

public class OrderDetailViewModel
{
    public int OrderId { get; set; }
    public string OrderStatus { get; set; } = null!;

    public int InvoiceId { get; set; }

    public DateTime? Paidon { get; set; }

    public DateTime? Placeon { get; set; }

    public DateTime? CompletedTime { get; set; }

    public DateTime? Modifieddate { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerMobile { get; set; } = null!;

    public string? CustomerEmail { get; set; }

    public int? TotalPerson { get; set; }

    public List<string> TableName { get; set; } = null!;

    public string SectionName { get; set; } = null!;

    public string PaymentMode {get;set;} = "Pending";

    public List<OrderItemViewModel> ItemList { get; set; }

    public List<OrderTaxViewModel> TaxList { get; set; }

}

public class OrderItemViewModel
{
    public string ItemName { get; set; } = null!;

    public List<OrderModifierViewModel> ModifierList { get; set; }

    public int? ItemQuantity { get; set; }

    public short? ItemPrice { get; set; }

}

public class OrderModifierViewModel
{
    public string ModifierItemName { get; set; } = null!;

    public int? ModifierItemQuantity { get; set; }

    public short? ModifierItemPrice { get; set; }
}

public class OrderTaxViewModel
{   
    public int TaxId { get; set; }
    public string TaxName { get; set; } = null!;

    public string TaxType { get; set; }

    public decimal TaxAmount { get; set; }
}
