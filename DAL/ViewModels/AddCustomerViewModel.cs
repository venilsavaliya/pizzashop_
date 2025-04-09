namespace DAL.ViewModels;

public class AddCustomerViewModel
{   
    public int? CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string Mobile { get; set; } = null!;

    public int TotalPerson { get; set; } = 1;

    public int TotalVisit { get; set; }=1;
  
}
