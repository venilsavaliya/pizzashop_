
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace pizzashop.presentation.Controllers;

public class AuthController : Controller
{
    private readonly IUserService _userService;

    private readonly IAuthService _authservice;

    private readonly IWebHostEnvironment _env;

    private readonly IEmailService _emailService;

    private readonly INotyfService _notyfy;

    private readonly IJwtService _jwtService;

    public AuthController(IUserService userService, IAuthService authService, IWebHostEnvironment env, IEmailService emailService, INotyfService notyfy, IJwtService jwtService, IAdminService adminservice)
    {
        _userService = userService;
        _authservice = authService;
        _env = env;
        _emailService = emailService;
        _notyfy = notyfy;
        _jwtService = jwtService;
    }

    // Access Denied Page
    public IActionResult AccessDenied()
    {
        return View();
    }

    #region Login
    // GET : Auth/Login
    public IActionResult Login()
    {
        if (Request.Cookies["jwt"] != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST : Auth/Login
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Authenticate the user 
        var AuthResponse = _authservice.AuthenticateUser(model);

        // if user is not authenticated then return to the login page
        if (!AuthResponse.Success)
        {
            // ModelState.AddModelError("InvalidCredentials", AuthResponse.Message ?? "Invalid Credentials");

            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = "Invalid email or password.";
            return View(model);
        }

        // if remember me functionality is checked than we store token for 7 days
        if (model.RememberMe)
        {
            Response.Cookies.Append("jwt", AuthResponse.Token ?? "", new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }
        else
        {
            // if remember me not checked than we dont give cookie expire time it automatically remove when user close browser 
            Response.Cookies.Append("jwt", AuthResponse.Token ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = "Login Successfully!";

        return RedirectToAction("Index", "Home");
    }

    #endregion


    // GET : Auth/ForgotPassword
    public IActionResult ForgotPassword(string email)
    {
        var forgetpasswordmodel = new ForgotPasswordViewModel
        {
            Email = email
        };

        return View(forgetpasswordmodel);
    }

    // POST : Auth/ForgotPassword
    [HttpPost]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View("ForgotPassword", model);
        }

        var AuthResponse = _authservice.ForgotPassword(model.Email);

        if (AuthResponse.Result.Success)
        {
            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = "Email Sent Successfully!";
        }
        else
        {
            ModelState.AddModelError("InvalidEmail", AuthResponse.Result.Message ?? "Could Not Send Email");
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Result.Message;
        }

        return View("ForgotPassword", model);
    }


    // GET : Auth/ResetPassword

    public IActionResult ResetPassword(string token)
    {
        if (_jwtService.IsTokenExpired(token))
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = "Your Link is Expired!";
            return RedirectToAction("LinkExpired", "Error");
        }

        var email = _jwtService.GetEmailDetailsFromToken(token);
        var model = new ResetPasswordviewModel
        {
            Email = email,
            Token = token
        };
        return View(model);
    }

    // POST : Auth/ResetPassword
    [HttpPost]
    public IActionResult ResetPassword(ResetPasswordviewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var AuthResponse = _authservice.ResetPassword(model);
        var response = _jwtService.AddTokenToDb(model.Token).Result;

        if (AuthResponse.Success && response.Success)
        {
            Console.WriteLine("Password Reset Successfully");
            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = "Password Reset Successfully!";
            return RedirectToAction("Login", "Auth");
        }
        else
        {
            ModelState.AddModelError("CustomeError", AuthResponse.Message ?? "Could Not Change Password");
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
        }

        return View();
    }


    // GET: Auth/Logout
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = "Logout Successfully!";
        return RedirectToAction("Login", "Auth");
    }

}