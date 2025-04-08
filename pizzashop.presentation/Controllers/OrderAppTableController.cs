using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class OrderAppTableController:BaseController
{

    private readonly IOrderAppTableService _OrderAppTableService;

    public OrderAppTableController(IJwtService jwtService,IUserService userService,IAdminService adminservice,IOrderAppTableService OrderAppTableService,IAuthorizationService authservice):base(jwtService,userService,adminservice,authservice)
    {
        _OrderAppTableService = OrderAppTableService;
    }

    public IActionResult Index()
    {  
       
        ViewBag.active = "OrderAppTable";
        return View();
        
    }

    public IActionResult GetSectionList()
    {
        var SectionTableList = _OrderAppTableService.GetOrderAppTableAndSectionList().Result;

        return PartialView("~/Views/OrderAppTable/_SectionTableList.cshtml", SectionTableList);
    }


}
