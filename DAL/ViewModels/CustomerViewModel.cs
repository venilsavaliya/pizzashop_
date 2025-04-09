namespace DAL.ViewModels;

public class CustomerViewModel
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string Mobile { get; set; } = null!;

    public int TotalVisit { get; set; } = 1;

    public DateTime JoinDate { get; set; }
}
