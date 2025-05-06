using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace pizzashop.presentation.Controllers;

[Authorize(Roles = "Account Manager,Admin,Chef")]
public class OrderAppKOTController:OrderAppBaseController
{
    private readonly IOrderAppKOTService _orderAppKOTService;
    public OrderAppKOTController(IJwtService jwtService,IUserService userService,IAdminService adminservice,BLL.Interfaces.IAuthorizationService authservice,IOrderAppKOTService orderAppKOTService):base(jwtService,userService,adminservice,authservice)
    {
        _orderAppKOTService = orderAppKOTService;
    }
    
    public IActionResult Index()
    {  
        ViewBag.active = "KOT";

        var model = _orderAppKOTService.GetCategoriesAsync().Result;
        
        return View(model);
    }

    // Get : Get List Of Categories

    public async Task<IActionResult> GetCategories()
    {
        var categories = await _orderAppKOTService.GetCategoriesAsync();
        return Json(new { data = categories });
    }

    public async Task<IActionResult> GetOrderDetails(int categoryid,bool isPending )
    {
        var orderDetails = await _orderAppKOTService.GetOrderDetailsAsync(categoryid, isPending);
        return PartialView("~/Views/OrderAppKOT/_OrderDetailCard.cshtml", orderDetails);
    }

    public async Task<IActionResult> GetOrderitemList(int orderid,bool isPending)
    {
        var orderDetails = await _orderAppKOTService.GetOrderitemListAsync(orderid, isPending);
        return PartialView("~/Views/OrderAppKOT/_OrderItems.cshtml", orderDetails);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderQuantity([FromBody] UpdateOrderRequestViewModel request)
    {
        var response = await _orderAppKOTService.UpdateOrderQuantityAsync(request.Items, request.MarkasPrepared);
        return Json(new { success = response.Success , message = response.Message });
    }
    [HttpPost]
    public async Task<IActionResult> UpdateServedQuantity(List<DishItemServeQuantityViewModel> items)
    {  
        var response = await _orderAppKOTService.UpdateServedQuantityAsync(items);

        return Json(new { success = response.Success, message = response.Message });
    }
}
