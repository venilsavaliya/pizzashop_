using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class OrderAppWaitingListController:OrderAppBaseController
{   
    private readonly IOrderAppWaitingListService _waitingservice;
    public OrderAppWaitingListController(IJwtService jwtService,IUserService userService,IAdminService adminservice,BLL.Interfaces.IAuthorizationService authservice,IOrderAppWaitingListService waitingservice):base(jwtService,userService,adminservice,authservice)
    {
        _waitingservice=waitingservice;
    }
    
    public IActionResult Index()
    {  
        var model = _waitingservice.GetSectionList().Result;
        ViewBag.active = "WaitingList";
        return View(model);
    }

    // Add New Waiting Token 

    public async Task<IActionResult> AddWaitingToken(AddEditWaitingTokenViewModel model)
    {
        var response = await _waitingservice.AddWaitingToken(model);
        return Json(new{success=response.Success , message = response.Message});
    }

    // Get List of Waiting Token 

    public IActionResult GetWaitingList(int sectionid =0)
    {
        var model = _waitingservice.GetWaitingTokenList(sectionid).Result;

        return PartialView("~/Views/OrderAppWaitingList/_waitingList.cshtml",model);
    }
    // Get List of Waiting Token 

    public IActionResult GetSectionList()
    {
        var model = _waitingservice.GetSectionList().Result;

        return PartialView("~/Views/OrderAppWaitingList/_sectionList.cshtml",model);
    }
}
