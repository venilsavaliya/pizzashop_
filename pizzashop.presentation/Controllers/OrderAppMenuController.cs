using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class OrderAppMenuController:OrderAppBaseController
{
    public OrderAppMenuController(IJwtService jwtService,IUserService userService,IAdminService adminservice,BLL.Interfaces.IAuthorizationService authservice):base(jwtService,userService,adminservice,authservice)
    {
        
    }
    
    public IActionResult Index()
    {  
        ViewBag.active = "Menu";
        return View();
    }
}
