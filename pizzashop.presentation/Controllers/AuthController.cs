
using System.Security.Cryptography.X509Certificates;
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

    public AuthController(IUserService userService, IAuthService authService, IWebHostEnvironment env, IEmailService emailService)
    {
        _userService = userService;
        _authservice = authService;
        _env = env;
        _emailService = emailService;
    }



    // GET : Auth/Login
    public IActionResult Login()
    {
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
            ModelState.AddModelError("InvalidCredentials", AuthResponse.Message ?? "Invalid Credentials");
            return View(model);
        }

        // if user is authenticated than we store the token in cookies and redirect to home page


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

            // get user name by email address
            var user = _userService.GetUserDetailByemail(model.Email);

            Response.Cookies.Append("UserName", user.UserName, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            Response.Cookies.Append("email", model.Email, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

        }

        TempData["SuccessMessage"] = "Logged in successfully!";

        return RedirectToAction("Index", "Home");
    }


    // GET : Auth/ForgotPassword
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // POST : Auth/ForgotPassword
    [HttpPost]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var AuthResponse = _authservice.ForgotPassword(model.Email);

        if (AuthResponse.Result.Success)
        {
            // TempData["SuccessMessage"] = "Email Sent Successfully";
            ViewBag.Message = "Email Sent Successfully";
        }
        else{
            ModelState.AddModelError("InvalidEmail", AuthResponse.Result.Message ?? "Could Not Send Email");
        }

        return View();
    }


    // GET : Auth/ResetPassword

    public IActionResult ResetPassword()
    {
        return View();
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

        if (AuthResponse.Success)
        {
            Console.WriteLine("Password Reset Successfully");
        }
        else
        {
            ModelState.AddModelError("CustomeError", AuthResponse.Message ?? "Could Not Change Password");
        }

        return View();
    }


    // GET: Auth/Logout
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        Response.Cookies.Delete("email");
        Response.Cookies.Delete("UserName");
        Response.Cookies.Delete("ProfileUrl");

        return RedirectToAction("Login", "Auth");
    }

}