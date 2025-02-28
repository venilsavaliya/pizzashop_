
using System.Security.Cryptography.X509Certificates;
using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace pizzashop.presentation.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    private readonly IAuthService _authservice;

    private readonly IWebHostEnvironment _env;

    private readonly IEmailService _emailService;

    public AdminController(IAdminService adminService, IAuthService authService, IWebHostEnvironment env, IEmailService emailService)
    {
        _adminService = adminService;
        _authservice = authService;
        _env = env;
        _emailService = emailService;
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
        return View(rolesandpermission);
    }

    // GET : Admin/Permissions


    [HttpPost]
    public IActionResult SavePermissions(RolesPermissionListViewModel permissions)
    {
        var AuthResponse = _adminService.SavePermission(permissions.Permissionlist).Result;

        if(AuthResponse.Success){
            TempData["SuccessMessage"]="Permission Change Successfully";
        }

        // Redirect to the same page or another page
        return RedirectToAction("Index","Home");
    }











}