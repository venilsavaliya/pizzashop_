using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace pizzashop.presentation.Controllers;

[Authorize(Roles = "Account Manager,Admin")]
public class OrderAppBaseController : Controller
{
  private string username;

  private string userprofile;

  private string email;

  private string role;

  private readonly IJwtService _jwtservice;

  private readonly IUserService _userService;

  private readonly IAdminService _adminservice;

  private readonly BLL.Interfaces.IAuthorizationService _authservice;
  public OrderAppBaseController(IJwtService jwtService, IUserService userService, IAdminService adminservice, BLL.Interfaces.IAuthorizationService authservice)
  {
    _jwtservice = jwtService;

    _userService = userService;

    _adminservice = adminservice;

    _authservice = authservice;
  }

  public override void OnActionExecuting(ActionExecutingContext context)
  {
    base.OnActionExecuting(context);
    var token = Request.Cookies["jwt"];
    email = _jwtservice.GetEmailDetailsFromToken(token);

    Userdetail user = _userService.GetUserDetailByemail(email);
    username = user.UserName;
    userprofile = user.Profile;
    role = _adminservice.GetRoleNameByRoleId(user.RoleId.ToString());

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
