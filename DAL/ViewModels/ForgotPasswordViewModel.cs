namespace DAL.ViewModels;
using System.ComponentModel.DataAnnotations;

public class ForgotPasswordViewModel
{
    [EmailAddress(ErrorMessage = "Please Enter Valid Email Address.")]
    public string Email { get; set; } = null!;
    public bool isSubmited { get; set; } = false;
}
