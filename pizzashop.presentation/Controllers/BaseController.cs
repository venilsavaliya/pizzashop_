using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace pizzashop.presentation.Controllers;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class BaseController : Controller
{

    private string username;

    private string userprofile;

    private string email;

    private string role;

    private readonly IJwtService _jwtservice;

    private readonly IUserService _userService;

    private readonly IAdminService _adminservice;

    public BaseController(IJwtService jwtService,IUserService userService,IAdminService adminservice)
    {
        _jwtservice = jwtService;

        _userService = userService;

        _adminservice = adminservice;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {   
        base.OnActionExecuting(context);
        var token = Request.Cookies["jwt"];
        email =  _jwtservice.GetEmailDetailsFromToken(token);

        Userdetail user =  _userService.GetUserDetailByemail(email);
        username = user.UserName;
        userprofile = user.Profile;
        role =  _adminservice.GetRoleNameByRoleId(user.RoleId.ToString());

        ViewBag.Username = username;
        ViewBag.email = email;
        ViewBag.Profile = userprofile;
        ViewBag.Role = role;
    }

    public string GetuserName()
    {
        return username;
    }
    public string GetUserProfile()
    {
        return userprofile;
    }
    public string GetUserEmail()
    {
        return email;
    }

    public string GetUserRole()
    {
        return role;
    }
}
