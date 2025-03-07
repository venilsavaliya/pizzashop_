using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace pizzashop.presentation.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : BaseController
{
    private readonly IAdminService _adminService;

    private readonly IAuthService _authservice;

    private readonly IWebHostEnvironment _env;

    private readonly IEmailService _emailService;

    private readonly INotyfService _notyf;

    public AdminController(IAdminService adminService, IAuthService authService, IWebHostEnvironment env, IEmailService emailService, INotyfService notyf, IJwtService jwtService, IUserService userService, IAdminService adminservice) : base(jwtService, userService, adminservice)
    {
        _adminService = adminService;
        _authservice = authService;
        _env = env;
        _emailService = emailService;
        _notyf = notyf;
    }


    // GET : Admin/Roles

    public IActionResult Roles()
    {
        // getting all roles from services
        var roles = _adminService.GetAllRoles();

        return View(roles);
    }

    public IActionResult Permission(string id)
    {
        var rolesandpermission = _adminService.GetRolespermissionsByRoleId(id);
        ViewBag.RoleName = _adminService.GetRoleNameByRoleId(id);
        return View(rolesandpermission);
    }

    // GET : Admin/Permissions

    [HttpPost]
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



}