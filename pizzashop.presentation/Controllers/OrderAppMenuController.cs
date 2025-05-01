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
        ViewBag.orderid = orderid;
        return View(model);
    }

    public async Task<List<MenuOrderItemViewModel>> GetMenuOrderItemList(int orderid)
    {
        var model = await _orderAppMenuservice.GetOrderDetailByOrderId(orderid);

        return model.OrderItems;
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

    //Get Order Status

    public string GetOrderStatus(int orderid)
    {
        return _orderAppMenuservice.GetOrderStatus(orderid);
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

        return Json(new { success = false, message = "Error Occured!" });

    }

    //Get Ready Quantiy Of Item 
    public int GetReadyQuantityOfItem(int id = 0)
    {
        return _orderAppMenuservice.GetReadyQuantityOfItem(id);
    }

    public async Task<IActionResult> GetOrderCustomerDetail(int orderid)
    {
        var model = await _orderAppMenuservice.GetCustomerDetailsByOrderId(orderid);
        return PartialView("~/Views/OrderAppMenu/_CustomerDetailForm.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCustomerDetail(OrderCustomerDetailViewModel model)
    {
        var response = await _orderAppMenuservice.SaveCustomerDetail(model);
        return Json(new { message = response.Message, success = response.Success });
    }

    public async Task<IActionResult> SaveOrderInstruction(InstructionViewModel model)
    {
        var response = await _orderAppMenuservice.SaveOrderInstruction(model);
        return Json(new { message = response.Message, success = response.Success });
    }

    // Get Instruction Detail For Order Or Item
    public async Task<IActionResult> GetInstruction(int orderid = 0, int dishid = 0, int index = 0, string Instruction = "")
    {
        InstructionViewModel model;

        model = await _orderAppMenuservice.GetInstruction(dishid, orderid, index, Instruction);
        ModelState.Clear();
        return PartialView("~/Views/OrderAppMenu/_OrderInstructionForm.cshtml", model);
    }

    // Save Instruction For Order Or Item
    [HttpPost]
    public async Task<IActionResult> SaveInstruction(InstructionViewModel model)
    {
        var response = await _orderAppMenuservice.SaveOrderInstruction(model);

        return Json(new { message = response.Message, success = response.Success });
    }

    [HttpPost]

    public async Task<IActionResult> CompleteOrder(string model)
    {
        // deserialize the model to get viewmodel
        if (!string.IsNullOrEmpty(model))
        {
            SaveOrderItemsViewModel Model = JsonConvert.DeserializeObject<SaveOrderItemsViewModel>(model);

            var response = await _orderAppMenuservice.CompleteOrder(Model);

            return Json(new { success = response.Success, message = response.Message });
        }

        return Json(new { message = "Error Occured!", success = false });
    }

    public async Task<IActionResult> CancelOrder(int orderid)
    {
        var response = await _orderAppMenuservice.CancelOrder(orderid);
        return Json(new {message=response.Message, success = response.Success });
    }
}
