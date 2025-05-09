using BLL.Interfaces;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;
namespace pizzashop.presentation.Controllers;

public class AdminController : BaseController
{
    private readonly IAdminService _adminService;

    private readonly IAuthService _authservice;

    private readonly IWebHostEnvironment _env;

    private readonly IEmailService _emailService;

    public AdminController(IAdminService adminService, IAuthService authService, IWebHostEnvironment env, IEmailService emailService, IJwtService jwtService, IUserService userService, IAdminService adminservice,BLL.Interfaces.IAuthorizationService authservice) : base(jwtService, userService, adminservice,authservice)
    {
        _adminService = adminService;
        _authservice = authService;
        _env = env;
        _emailService = emailService;
    }


    // GET : Admin/Roles
    [AuthorizePermission(PermissionName.RolesAndPermission, ActionPermission.CanView)]
    public IActionResult Roles()
    {
        // getting all roles from services
        var roles = _adminService.GetAllRoles();

        return View(roles);
    }
    
    [AuthorizePermission(PermissionName.RolesAndPermission, ActionPermission.CanAddEdit)]
    public IActionResult Permission(string id)
    {
        var rolesandpermission = _adminService.GetRolespermissionsByRoleId(id);

        ViewBag.RoleName = _adminService.GetRoleNameByRoleId(id);

        return View(rolesandpermission);
    }

    // GET : Admin/Permissions

    [HttpPost]
    [AuthorizePermission(PermissionName.RolesAndPermission, ActionPermission.CanAddEdit)]
    public IActionResult SavePermissions(RolesPermissionListViewModel permissions)
    {
        var AuthResponse = _adminService.SavePermission(permissions).Result;

        if (AuthResponse.Success)
        {
            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = AuthResponse.Message;

            return RedirectToAction("Roles", "Admin");
        }

        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = AuthResponse.Message;

        return RedirectToAction("Roles", "Admin");
    }


    public async Task<IActionResult> GetDashboardData(DateTime startdate, DateTime enddate)
    {
        var data = await _adminService.GetDashboardData(startdate,enddate);
        return PartialView("_DashboardPartial",data);
    }


}