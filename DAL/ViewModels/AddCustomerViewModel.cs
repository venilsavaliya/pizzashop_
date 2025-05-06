using System.ComponentModel.DataAnnotations;
namespace DAL.ViewModels;

public class AddCustomerViewModel
{   
    public int? CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    [Range(1, 30, ErrorMessage = "Please select a valid number of persons.")]
    public int TotalPerson { get; set; } = 1;
    public int TotalVisit { get; set; }=1;
  
}
