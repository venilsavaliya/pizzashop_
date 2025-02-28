
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

    public AuthController(IUserService userService, IAuthService authService, IWebHostEnvironment env, IEmailService emailService, INotyfService notyfy, IJwtService jwtService)
    {
        _userService = userService;
        _authservice = authService;
        _env = env;
        _emailService = emailService;
        _notyfy = notyfy;
        _jwtService = jwtService;
    }



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
            ModelState.AddModelError("InvalidCredentials", AuthResponse.Message ?? "Invalid Credentials");
            
            TempData["ErrorMessage"] = "Invalid email or password.";
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

        _notyfy.Success("Login Successfull", 3);

        return RedirectToAction("Index", "Home");
    }


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
            TempData["SuccessMessage"] = "Email Sent Successfully";
            ViewBag.Message = "Email Sent Successfully";
        }
        else
        {
            ModelState.AddModelError("InvalidEmail", AuthResponse.Result.Message ?? "Could Not Send Email");
            TempData["ErrorMessage"] = "Could Not Send Email";
        }

        return View("ForgotPassword", model);
    }


    // GET : Auth/ResetPassword

    public IActionResult ResetPassword(string token)
    {
        if(_jwtService.IsTokenExpired(token)){
            return RedirectToAction("LinkExpired","Error");
        }

        var email = _jwtService.GetEmailDetailsFromToken(token);
        var model =  new ResetPasswordviewModel{
            Email= email,
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

        if (AuthResponse.Success)
        {
            Console.WriteLine("Password Reset Successfully");
            TempData["SuccessMessage"] = "Password Reset Successfully";
            return RedirectToAction("Login", "Auth");
        }
        else
        {
            ModelState.AddModelError("CustomeError", AuthResponse.Message ?? "Could Not Change Password");
            TempData["ErrorMessage"] = "Could Not Change Password";
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