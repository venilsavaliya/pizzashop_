using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class OrderAppWaitingListController:OrderAppBaseController
{
    public OrderAppWaitingListController(IJwtService jwtService,IUserService userService,IAdminService adminservice,BLL.Interfaces.IAuthorizationService authservice):base(jwtService,userService,adminservice,authservice)
    {
        
    }
    
    public IActionResult Index()
    {  
        ViewBag.active = "WaitingList";
        return View();
    }
}
