namespace DAL.ViewModels;

public class CustomerHistoryDetailViewModel
{
    public int CustomerId { get; set; } 

    public string Name { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public decimal MaxOrderAmount { get; set; }

    public decimal AverageOrderAmount { get; set; }

    public int TotalVisit { get; set; }

    public DateTime JoinDate { get; set; }

    public IEnumerable<CustomerHistoryViewModel>? Items { get; set; }
}
