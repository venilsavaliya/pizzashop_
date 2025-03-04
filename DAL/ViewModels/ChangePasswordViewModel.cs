namespace DAL.ViewModels;
using System.ComponentModel.DataAnnotations;
public class ChangePasswordViewModel
{
    public string OldPassword { get; set; } = null!;

    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string NewPassword { get; set; } = null!;
    
    [Compare("NewPassword", ErrorMessage = "Password and Confirm Password must match")]
    public string ConfirmPassword { get; set; } = null!;
}
