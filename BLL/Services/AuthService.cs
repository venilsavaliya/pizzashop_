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
        // Check For User is existed in DB or Not
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);

        // if user is present in db then process further
        if (existingUser != null)
        {
            if (PasswordService.VerifyPassword(model.Password, existingUser.Password))
            {
                // --- User Logged In Successfully ---

                // Getting Role of the user from the database
                var roleid = _context.Userdetails.FirstOrDefault(u => u.UserId == existingUser.Id);
                string role = _context.Roles.FirstOrDefault(u => u.Roleid == roleid.RoleId)?.Name ?? "User";

                // setting jwt token which have email and role of the user
                var token = _jwtService.GenerateJwtToken(existingUser.Email, role);

                // return the newly generated jwt token with message
                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    Message = "User Logged In Succesfully"
                };
            }
            else
            {
                return new AuthResponse
                {
                    Success = false,
                    Token = null,
                    Message = "Invald Email or Password"
                };
            }
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Token = null,
                Message = "Invald Email or Password"
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
        var jwtToken = _jwtService.GenerateJwtToken(email,"",1);
        var uri = _urlHelper.Action("ResetPassword", "Auth", new { token = jwtToken}, "http");
        htmltemplate = htmltemplate.Replace("resetlink",uri);

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
            return new AuthResponse{
                Success = false,
                Token = null,
                Message = "User Not Found"
            };
        }

        // here we check for password and confirm password are same or not
        if(model.Password != model.ConfirmPassword)
        {
            return new AuthResponse{
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

        return new AuthResponse{
            Success = true,
            Token = null,
            Message = "Password has been reset successfully."
        };

    }

}
