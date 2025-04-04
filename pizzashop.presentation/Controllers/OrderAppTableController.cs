using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class OrderAppTableController:BaseController
{

    private readonly ITaxService _taxservice;

    public OrderAppTableController(IJwtService jwtService,IUserService userService,IAdminService adminservice,ITaxService taxservice,IAuthorizationService authservice):base(jwtService,userService,adminservice,authservice)
    {
        _taxservice = taxservice;
    }
    // [AuthorizePermission(PermissionName.TaxAndFees, ActionPermission.CanView)]
    public IActionResult Index()
    {  
        ViewBag.active = "Tables";
        return View();
    }


}
