namespace DAL.ViewModels;

public class OrderViewModel
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string CustomerName { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string PaymentMode { get; set; } = null!;

    public short? Rating { get; set; }

    public decimal TotalAmount { get; set; }

}
