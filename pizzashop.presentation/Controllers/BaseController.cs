using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DAL.Constants;
using DAL.ViewModels;

namespace pizzashop.presentation.Controllers;

[Authorize(Roles = "Admin,Account Manager")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class BaseController : Controller
{

    private string? username;

    private string? userprofile;

    private string? email;

    private string? role;

    private readonly IJwtService _jwtservice;

    private readonly IUserService _userService;

    private readonly IAdminService _adminservice;

    private readonly BLL.Interfaces.IAuthorizationService _authservice;

    public BaseController(IJwtService jwtService, IUserService userService, IAdminService adminservice, BLL.Interfaces.IAuthorizationService authservice)
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
        userprofile = user.Profile??"/images/icons/person-fill.svg";
        role = _adminservice.GetRoleNameByRoleId(user.RoleId.ToString()??"0");

        ViewBag.Username = username;
        ViewBag.email = email;
        ViewBag.Profile = userprofile;
        ViewBag.Role = role;


        ViewBag.CanUserView = _authservice.HasPermission(PermissionName.Users, ActionPermission.CanView);
        ViewBag.CanMenuView = _authservice.HasPermission(PermissionName.Menu, ActionPermission.CanView);
        ViewBag.CanTaxAndFeesView = _authservice.HasPermission(PermissionName.TaxAndFees, ActionPermission.CanView);
        ViewBag.CanOrdersView = _authservice.HasPermission(PermissionName.Orders, ActionPermission.CanView);
        ViewBag.CanCustomersView = _authservice.HasPermission(PermissionName.Customers, ActionPermission.CanView);
        ViewBag.CanTableAndSectionView = _authservice.HasPermission(PermissionName.TableAndSection, ActionPermission.CanView);
        ViewBag.CanRolesAndPermissionView = _authservice.HasPermission(PermissionName.RolesAndPermission, ActionPermission.CanView);


        ViewBag.CanUserAddEdit = _authservice.HasPermission(PermissionName.Users, ActionPermission.CanAddEdit);
        ViewBag.CanMenuAddEdit = _authservice.HasPermission(PermissionName.Menu, ActionPermission.CanAddEdit);
        ViewBag.CanTaxAndFeesAddEdit = _authservice.HasPermission(PermissionName.TaxAndFees, ActionPermission.CanAddEdit);
        ViewBag.CanOrdersAddEdit = _authservice.HasPermission(PermissionName.Orders, ActionPermission.CanAddEdit);
        ViewBag.CanCustomersAddEdit = _authservice.HasPermission(PermissionName.Customers, ActionPermission.CanAddEdit);
        ViewBag.CanTableAndSectionAddEdit = _authservice.HasPermission(PermissionName.TableAndSection, ActionPermission.CanAddEdit);
        ViewBag.CanRolesAndPermissionAddEdit = _authservice.HasPermission(PermissionName.RolesAndPermission, ActionPermission.CanAddEdit);


        ViewBag.CanUserDelete = _authservice.HasPermission(PermissionName.Users, ActionPermission.CanDelete);
        ViewBag.CanMenuDelete = _authservice.HasPermission(PermissionName.Menu, ActionPermission.CanDelete);
        ViewBag.CanTaxAndFeesDelete = _authservice.HasPermission(PermissionName.TaxAndFees, ActionPermission.CanDelete);
        ViewBag.CanOrdersDelete = _authservice.HasPermission(PermissionName.Orders, ActionPermission.CanDelete);
        ViewBag.CanCustomersDelete = _authservice.HasPermission(PermissionName.Customers, ActionPermission.CanDelete);
        ViewBag.CanTableAndSectionDelete = _authservice.HasPermission(PermissionName.TableAndSection, ActionPermission.CanDelete);
        ViewBag.CanRolesAndPermissionDelete = _authservice.HasPermission(PermissionName.RolesAndPermission, ActionPermission.CanDelete);


    }

    public string GetuserName()
    {
        return username??Constants.Unknown;
    }
    public string GetUserProfile()
    {
        return userprofile??Constants.Unknown;
    }
    public string GetUserEmail()
    {
        return email??Constants.Unknown;
    }
    public string GetUserRole()
    {
        return role??Constants.Unknown;
    }
}
