using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

[Authorize(Roles = "Account Manager,Admin")]
public class OrderAppWaitingListController : OrderAppBaseController
{
    private readonly IOrderAppWaitingListService _waitingservice;
    public OrderAppWaitingListController(IJwtService jwtService, IUserService userService, IAdminService adminservice, BLL.Interfaces.IAuthorizationService authservice, IOrderAppWaitingListService waitingservice) : base(jwtService, userService, adminservice, authservice)
    {
        _waitingservice = waitingservice;
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
        return Json(new { success = response.Success, message = response.Message }); 
    }
    public async Task<IActionResult> AddEditWaitingToken(AddEditWaitingTokenViewModel model)
    {
        var response = await _waitingservice.AddWaitingToken(model);
        return Json(new { success = response.Success, message = response.Message });
    }


    public async Task<IActionResult> GetAddEditWaitingTokenForm(int tokenid = 0, int activeSectionId = 0, string formId = "")
    {
        var model = await _waitingservice.GetAddEditWaitingTokenDetail(tokenid);
        if (tokenid == 0)
        {
            model.Sectionid = activeSectionId;
        }
        if (!string.IsNullOrEmpty(formId))
        {
            ViewData["FormId"] = formId;
        }
        return PartialView("~/Views/OrderAppWaitingList/_AddEditWaitingTokenForm.cshtml", model);
    }

    // Get List of Waiting Token 

    public async Task<IActionResult> GetWaitingList(int sectionid = 0)
    {
        var model = await _waitingservice.GetWaitingTokenList(sectionid);

        return PartialView("~/Views/OrderAppWaitingList/_waitingList.cshtml", model);
    }
    // Get List of Waiting Token 

    public async Task<IActionResult> GetSectionList()
    {
        var model = await _waitingservice.GetSectionList();

        return PartialView("~/Views/OrderAppWaitingList/_sectionList.cshtml", model);
    }

    // Delte Waiting Token
    [HttpPost]
    public async Task<IActionResult> DeleteWaitingToken(int TokenId)
    {
        var response = await _waitingservice.DeleteWaitingToken(TokenId);

        return Json(new { message = response.Message, success = response.Success });
    }

    // Get Available Tables List Based On SectionId

    public async Task<IActionResult> GetAvailableTableList(int SectionId)
    {
        var response = await _waitingservice.GetAvailableTableList(SectionId);

        return Ok(response);
    }
}
