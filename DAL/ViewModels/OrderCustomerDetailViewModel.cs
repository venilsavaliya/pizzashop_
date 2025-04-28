using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class OrderCustomerDetailViewModel
{
    public int OrderId {get;set;}

    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    // [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
    [MinLength(10, ErrorMessage = "Mobile number must be 10 digits")]
    [MaxLength(10, ErrorMessage = "Mobile number must be 10 digits")]

    public string Mobile { get; set; } = null!;

    public int TotalPerson { get; set; } = 1;
}
