namespace DAL.ViewModels;

public partial class ResetPasswordviewModel
{

    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
