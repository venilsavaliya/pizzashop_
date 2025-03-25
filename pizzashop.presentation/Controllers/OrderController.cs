using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace pizzashop.presentation.Controllers;

public class OrderController:BaseController
{
    private readonly IOrderService _orderservice;

    public OrderController(IJwtService jwtService,IUserService userService,IAdminService adminservice,IAuthorizationService authservice,IOrderService orderservice):base(jwtService,userService,adminservice,authservice)
    {
        _orderservice = orderservice;
    }
    [AuthorizePermission(PermissionName.Orders, ActionPermission.CanView)]
    public IActionResult Index()
    {  
        ViewBag.active = "Order";
        return View();
    }

    // Return Partial View Of Filtered Order Table List
    [AuthorizePermission(PermissionName.Orders, ActionPermission.CanView)]
    public async Task<IActionResult> GetOrderList(string sortColumn="", string sortOrder="",int pageNumber = 1, int pageSize = 5, string searchKeyword = "",string status="",string startDate = null , string endDate = null)
    {   

        if(!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
        {
            var model =await _orderservice.GetOrderList( sortColumn,sortOrder,pageNumber, pageSize, searchKeyword,status,DateTime.Parse(startDate),DateTime.Parse(endDate));
            return PartialView("~/Views/Order/_OrderList.cshtml", model);
        }
        else
        {
            var model =await _orderservice.GetOrderList( sortColumn,sortOrder,pageNumber, pageSize, searchKeyword,status);
            return PartialView("~/Views/Order/_OrderList.cshtml", model);
        }

        
    }

    
}
