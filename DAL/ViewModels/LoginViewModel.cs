namespace DAL.ViewModels;
using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{   
   [EmailAddress(ErrorMessage = "Please Enter Valid Email Address.")]
    public string Email { get; set; } = null!;
    public  string Password { get; set; } = null!;
    public bool RememberMe { get; set; }=false;
}
 
