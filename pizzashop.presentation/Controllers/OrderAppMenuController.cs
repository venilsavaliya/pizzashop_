using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    public IActionResult Index()
    {
        ViewBag.active = "Menu";
        return View();
    }

    public IActionResult GetCategoryList()
    {
        var model = _menuservice.GetCategoryList();

        return PartialView("~/Views/OrderAppMenu/_CategoryList.cshtml", model);
    }

    public async Task<IActionResult> GetMenuitemListById(int catid, string searchkeyword,bool isfav)
    {
        var model =await _orderAppMenuservice.GetMenuItem(catid, searchkeyword,isfav);

        return PartialView("~/Views/OrderAppMenu/_Menuitems.cshtml", model);
    }
}
