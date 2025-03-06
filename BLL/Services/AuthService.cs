using System;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BLL.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    private readonly IUrlHelper _urlHelper;

    private readonly IJwtService _jwtService;

    private readonly IEmailService _emailservice;

    private readonly IHttpContextAccessor _httpcontext;
    private readonly IWebHostEnvironment _env;

    public AuthService(ApplicationDbContext context, IJwtService jwtService, IEmailService emailService,
                   IWebHostEnvironment env, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
    {
        _context = context;
        _jwtService = jwtService;
        _emailservice = emailService;
        _env = env;
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
    }


    public AuthResponse AuthenticateUser(LoginViewModel model)
    {
        try
        {
            // Check if the user exists in the database
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (existingUser == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Token = null,
                    Message = "Invalid Email or Password"
                };
            }

            // Verify the entered password with the stored hash
            if (!PasswordService.VerifyPassword(model.Password, existingUser.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Token = null,
                    Message = "Invalid Email or Password"
                };
            }

            // --- User Logged In Successfully ---

            // Fetching Role of the user
            var role = _context.Userdetails
                               .Where(u => u.UserId == existingUser.Id)
                               .Select(u => _context.Roles.FirstOrDefault(r => r.Roleid == u.RoleId).Name)
                               .FirstOrDefault() ?? "User";

            // Generate JWT token containing email and role
            var token = _jwtService.GenerateJwtToken(existingUser.Email, role);

            return new AuthResponse
            {
                Success = true,
                Token = token,
                Message = "User Logged In Successfully"
            };
        }
        catch (Exception ex)
        {
            // exception 
            Console.WriteLine($"Error in AuthenticateUser: {ex.Message}");

            return new AuthResponse
            {
                Success = false,
                Token = null,
                Message = "An unexpected error occurred. Please try again later."
            };
        }
    }


    public async Task<AuthResponse> ForgotPassword(string email)
    {

        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Token = null,
                Message = "User Not Found"
            };
        }

        // -- if user found than we send email to the user --

        string htmltemplate = System.IO.File.ReadAllText(_env.WebRootPath + "/HtmlTemplate/ResetPassword.html");
        var jwtToken = _jwtService.GenerateJwtToken(email, "", 1);
        var uri = _urlHelper.Action("ResetPassword", "Auth", new { token = jwtToken }, "http");
        htmltemplate = htmltemplate.Replace("resetlink", uri);

        await _emailservice.SendEmailAsync(email, "Reset Password", htmltemplate);

        return new AuthResponse
        {
            Success = true,
            Token = null,
            Message = "Reset Password Link Sent to Your Email"
        };


    }

    //  ---------------------------------- Reset Password Functionality ----------------------------------

    public AuthResponse ResetPassword(ResetPasswordviewModel model)
    {
        // fetching user from db
        var User = _context.Users.FirstOrDefault(u => u.Email == model.Email);

        if (User == null)
        {
            return new AuthResponse
            {
                Success = false,
                Token = null,
                Message = "User Not Found"
            };
        }

        // here we check for password and confirm password are same or not
        if (model.Password != model.ConfirmPassword)
        {
            return new AuthResponse
            {
                Success = false,
                Token = null,
                Message = "Password and Confirm Password does not match."
            };
        }

        // we are hashing the password so that hashed password store in db
        string hashedPassword = PasswordService.HashPassword(model.Password);

        User.Password = hashedPassword;
        _context.Users.Update(User);
        _context.SaveChanges();

        return new AuthResponse
        {
            Success = true,
            Token = null,
            Message = "Password has been reset successfully."
        };

    }

}
