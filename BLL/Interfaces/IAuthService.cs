using BLL.Models;
using DAL.ViewModels;
namespace BLL.Interfaces;

public interface IAuthService
{
    public AuthResponse AuthenticateUser(LoginViewModel model);

    public  Task<AuthResponse> ForgotPassword(string email);

    public AuthResponse ResetPassword(ResetPasswordviewModel model);
}
