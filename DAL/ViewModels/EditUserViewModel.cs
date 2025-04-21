namespace DAL.ViewModels;

using System.ComponentModel.DataAnnotations;
using DAL.Validators;
using Microsoft.AspNetCore.Http;

public partial class EditUserViewModel 
{
    public Guid? Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string UserName { get; set; } = null!;

    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string? Phone { get; set; }

    public string? Email { get; set; }

     [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; set; } = null!;

    public bool Status { get; set; }
    
   
    public string? Country { get; set; }="";

    [RequiredIfCountryPresent("Country")]
    public string? State { get; set; }="";

    [RequiredIfCountryPresent("Country")]
    public string? City { get; set; }="";

    public string? Address { get; set; }

    [RegularExpression(@"^\d{6}$", ErrorMessage = "zipcode must be exactly 6 digits.")]
    public string? Zipcode { get; set; }

    public string? RoleName { get; set; }

    public IFormFile? Profile { get; set; }

}