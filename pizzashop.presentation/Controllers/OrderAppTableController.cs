using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace pizzashop.presentation.Controllers;

[Authorize(Roles = "Account Manager,Admin")]
public class OrderAppTableController : BaseController
{

    private readonly IOrderAppTableService _OrderAppTableService;
    private readonly ISectionServices _sectioService;

    private readonly IOrderAppWaitingListService _waitingservice;

    public OrderAppTableController(IJwtService jwtService, IUserService userService, IAdminService adminservice, IOrderAppTableService OrderAppTableService, BLL.Interfaces.IAuthorizationService authservice, ISectionServices sectioService, IOrderAppWaitingListService waitingservice) : base(jwtService, userService, adminservice, authservice)
    {
        _OrderAppTableService = OrderAppTableService;
        _sectioService = sectioService;
        _waitingservice = waitingservice;
    }

    public IActionResult Index()
    {

        ViewBag.active = "Tables";
        var sectionList = _sectioService.GetSectionList();
        return View(sectionList);

    }

    public IActionResult GetSectionList()
    {
        var SectionTableList = _OrderAppTableService.GetOrderAppTableAndSectionList().Result;

        return PartialView("~/Views/OrderAppTable/_SectionTableList.cshtml", SectionTableList);
    }

    // Assign Table to Customer

    public async Task<IActionResult> AssignTable(TableAssignViewModel model)
    {
        var orderid = await _OrderAppTableService.AssignTableAsync(model);

        return Json(new { success = true, orderid });
    }

    // Get List of Waiting Token 

    public async Task<IActionResult> GetWaitingList(int sectionid = 0)
    {
        var model = await _waitingservice.GetWaitingTokenList(sectionid);

        return PartialView("~/Views/OrderAppTable/_WaitingList.cshtml", model);
    }

    // Get Order Id Of Table

    public int GetOrderIdOfTable(int tableid = 0)
    {
        var result = _OrderAppTableService.GetOrderIdOfTable(tableid);
        return result;
    }


}
