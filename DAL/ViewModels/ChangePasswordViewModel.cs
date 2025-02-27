namespace DAL.ViewModels;
using System.ComponentModel.DataAnnotations;
public class ChangePasswordViewModel
{
    public string OldPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!;
    
    [Compare("NewPassword", ErrorMessage = "Password and Confirm Password must match")]
    public string ConfirmPassword { get; set; } = null!;
}
