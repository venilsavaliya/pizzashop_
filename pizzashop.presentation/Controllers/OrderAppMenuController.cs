using BLL.Interfaces;
using BLL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace pizzashop.presentation.Controllers;

public class OrderAppMenuController : OrderAppBaseController
{
    private readonly IMenuServices _menuservice;

    private readonly IOrderAppMenuService _orderAppMenuservice;
    public OrderAppMenuController(IJwtService jwtService, IOrderAppMenuService orderAppMenuservice, IUserService userService, IAdminService adminservice, BLL.Interfaces.IAuthorizationService authservice, IMenuServices menuservice) : base(jwtService, userService, adminservice, authservice)
    {
        _menuservice = menuservice;
        _orderAppMenuservice = orderAppMenuservice;
    }

    public async Task<IActionResult> Index(int orderid)
    {
        var model = await _orderAppMenuservice.GetOrderDetailByOrderId(orderid);
        ViewBag.active = "Menu";
        return View(model);
    }

    public IActionResult GetCategoryList()
    {
        var model = _menuservice.GetCategoryList();

        return PartialView("~/Views/OrderAppMenu/_CategoryList.cshtml", model);
    }

    public async Task<IActionResult> GetMenuitemListById(int catid, string searchkeyword, bool isfav = false)
    {
        var model = await _orderAppMenuservice.GetMenuItem(catid, searchkeyword, isfav);

        return PartialView("~/Views/OrderAppMenu/_Menuitems.cshtml", model);
    }
    public async Task<IActionResult> GetModifierGroupListById(int itemid)
    {
        var model = await _orderAppMenuservice.GetModifierGroupsByItemId(itemid);

        return PartialView("~/Views/OrderAppMenu/_Menuitems.cshtml", model);
    }

    public async Task<IActionResult> ChangeStatusOfFavouriteItem(int itemid = 0)
    {
        var response = await _orderAppMenuservice.ChangeStatusOfFavouriteItem(itemid);

        return Json(new { message = response.Message, success = response.Success });
    }

    public async Task<IActionResult> GetModifierItemsOfMenuItem(int id = 0)
    {
        var response = await _orderAppMenuservice.GetModifierItemsOfMenuItem(id);

        return PartialView("~/Views/OrderAppMenu/_ModifierItemsList.cshtml", response);
    }

    [HttpPost]
    public IActionResult GetMenuOrderItemPartialView(MenuOrderItemViewModel model)
    {
        string modifiersJson = Request.Form["ModifierItems"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersJson))
        {
            model.ModifierItems = JsonConvert.DeserializeObject<List<ModifierItemNamePriceViewModel>>(modifiersJson);
        }

        return PartialView("~/Views/OrderAppMenu/_MenuOrderItem.cshtml", model);
    }

    public async Task<IActionResult> SaveOrder(string model)
    {
       
        // deserialize the model to get viewmodel
        if (!string.IsNullOrEmpty(model))
        {
            SaveOrderItemsViewModel Model = JsonConvert.DeserializeObject<SaveOrderItemsViewModel>(model);

            var response = await _orderAppMenuservice.SaveOrderAsync(Model);

            return Json(new { success = response.Success, message = response.Message });
        }

        return Json(new {success=false,message="Error Occured!"});


    }
}
