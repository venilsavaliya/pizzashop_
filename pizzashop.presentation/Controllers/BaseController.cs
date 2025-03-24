using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DAL.Constants;
using DAL.ViewModels;

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

    private readonly BLL.Interfaces.IAuthorizationService _authservice;

    public BaseController(IJwtService jwtService,IUserService userService,IAdminService adminservice,BLL.Interfaces.IAuthorizationService authservice)
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
        email =  _jwtservice.GetEmailDetailsFromToken(token); 

        Userdetail user =  _userService.GetUserDetailByemail(email);
        username = user.UserName;
        userprofile = user.Profile;
        role =  _adminservice.GetRoleNameByRoleId(user.RoleId.ToString());

        ViewBag.Username = username;
        ViewBag.email = email;
        ViewBag.Profile = userprofile;
        ViewBag.Role = role;

        ViewBag.CanUserView = _authservice.HasPermission(PermissionName.Users,ActionPermission.CanView);
        ViewBag.CanMenuView = _authservice.HasPermission(PermissionName.Menu,ActionPermission.CanView);
        ViewBag.CanTaxAndFeesView = _authservice.HasPermission(PermissionName.TaxAndFees,ActionPermission.CanView);
        ViewBag.CanOrdersView = _authservice.HasPermission(PermissionName.Orders,ActionPermission.CanView);
        ViewBag.CanCustomersView = _authservice.HasPermission(PermissionName.Customers,ActionPermission.CanView);
        ViewBag.CanTableAndSectionView = _authservice.HasPermission(PermissionName.TableAndSection,ActionPermission.CanView);
        ViewBag.CanRolesAndPermissionView = _authservice.HasPermission(PermissionName.RolesAndPermission,ActionPermission.CanView);
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
