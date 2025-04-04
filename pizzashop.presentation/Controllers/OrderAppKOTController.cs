using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

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

    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var orderDetails = await _orderAppKOTService.GetOrderDetailsAsync(orderId);
        return Json(new { data = orderDetails });
    }
}
