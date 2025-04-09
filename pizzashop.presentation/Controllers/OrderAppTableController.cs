using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class OrderAppTableController : BaseController
{

    private readonly IOrderAppTableService _OrderAppTableService;
    private readonly ISectionServices _sectioService;

    public OrderAppTableController(IJwtService jwtService, IUserService userService, IAdminService adminservice, IOrderAppTableService OrderAppTableService, IAuthorizationService authservice, ISectionServices sectioService) : base(jwtService, userService, adminservice, authservice)
    {
        _OrderAppTableService = OrderAppTableService;
        _sectioService = sectioService;
    }

    public IActionResult Index()
    {

        ViewBag.active = "OrderAppTable";
        var sectionList = _sectioService.GetSectionList();
        return View(sectionList);

    }

    public IActionResult GetSectionList()
    {
        var SectionTableList = _OrderAppTableService.GetOrderAppTableAndSectionList().Result;

        return PartialView("~/Views/OrderAppTable/_SectionTableList.cshtml", SectionTableList);
    }

    // Assign Table to Customer

    public IActionResult AssignTable(TableAssignViewModel model)
    {
        var result = _OrderAppTableService.AssignTableAsync(model).Result;

        return Json(new { success = result.Success, message = result.Message });
    }


}
