namespace pizzashop.presentation.Controllers;
using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using BLL.Services;
using BLL.Attributes;
using DAL.Constants;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using BLL.Models;

public class SectionController : BaseController
{
  private readonly ISectionServices _sectionservice;
  public SectionController(IJwtService jwtService, IUserService userService, IAdminService adminservice, ISectionServices sectionservice, IAuthorizationService authservice) : base(jwtService, userService, adminservice, authservice)
  {
    _sectionservice = sectionservice;
  }
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanView)]
  public IActionResult Index(int? id)
  {
    var sections = _sectionservice.GetSectionList();
    if (id == null)
    {
      id = sections.First().SectionId;
    }

    ViewBag.active = "Section";

    var model = new SectionAndTableViewModel
    {
      SelectedSection = id,
      Sections = sections,
      TableStatus = _sectionservice.GetTableStatusList(),
      Table = new AddTableViewmodel()
    };
    return View(model);
  }

  // Get : Return partial View Of Pagination Table List
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanView)]
  public IActionResult GetDiningTableList(int id, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
  {
    var sections = _sectionservice.GetSectionList().ToList();

    if (id == 0)
    {
      id = sections.First().SectionId;
    }

    var model = _sectionservice.GetDiningTablesListBySectionId(id, pageNumber, pageSize, searchKeyword);
    ViewBag.active = "Menu";

    return PartialView("~/Views/Section/_TableList.cshtml", model);
  }

  // GET : Section List {Partial View Return}
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanView)]
  public IActionResult GetSections(int? id)
  {
    var sections = _sectionservice.GetSectionList().ToList();

    if (id == null)
    {
      id = sections.First().SectionId;
    }

    var model = new SectionNameListViewModel
    {
      Sections = sections,
      SelectedSection = id
    };

    return PartialView("~/Views/Section/_SectionList.cshtml", model);
  }

  // GET : Get Json Data Of SectionLIst
  public IActionResult GetSectionListData()
  {
    var sections = _sectionservice.GetSectionList();
    return Json(sections);
  }

  // POST : Add New Section
  // [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanAddEdit)]
  // [HttpPost]
  // public IActionResult AddSection(SectionAndTableViewModel model)
  // {
  //   var response = _sectionservice.AddSection(model.Section).Result;

  //   return Json(new { message = response.Message, success = response.Success });


  // }
  // POST : Edit New Section
  // [HttpPost]
  // [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanAddEdit)]
  // public IActionResult EditSection(SectionAndTableViewModel model)
  // {
  //   var response = _sectionservice.EditSection(model.Section).Result;

  //   return Json(new { message = response.Message, success = response.Success });

  // }

  // Delete Section

  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanDelete)]
  [HttpPost]
  public IActionResult DeleteSection(int id)
  {
    var AuthResponse = _sectionservice.DeleteSection(id).Result;

    return Json(new { message = AuthResponse.Message, success = AuthResponse.Success });
  }

  // POST : Add New Table
  [HttpPost]
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanAddEdit)]
  public IActionResult AddTable(SectionAndTableViewModel model)
  {

    var response = _sectionservice.AddTable(model.Table).Result;

    return Json(new { message = response.Message, success = response.Success });

  }


  [HttpPost]
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanAddEdit)]
  public IActionResult EditTable(SectionAndTableViewModel model)
  {
    var response = _sectionservice.EditTable(model.Table).Result;

    return Json(new { message = response.Message, success = response.Success });

  }

  // Delete Table
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanDelete)]
  public IActionResult DeleteTable(int id)
  {
    var AuthResponse = _sectionservice.DeleteTable(id).Result;

    return Json(new { message = AuthResponse.Message, success = AuthResponse.Success });
  }

  // POST : Delete Multiple Tables
  [HttpPost]
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanDelete)]
  public IActionResult DeleteTables(List<TableMassDeleteViewModel> ids)
  {

    var AuthResponse = _sectionservice.DeleteTables(ids).Result;

    return Json(new { message = AuthResponse.Message, success = AuthResponse.Success });

  }

  // Get : Get Id of Table Status By Name

  public IActionResult GetStatusIdByName(string name)
  {
    var statusId = _sectionservice.GetTableStatusIdByName(name);
    return Json(new { statusId = statusId });
  }


  // Get AddEdit Table Form Partial View

  public async Task<IActionResult> GetAddEditTableForm(int id = 0, int activesectionid = 0)
  {

    var model = await _sectionservice.GetTableDetailById(id);
    if (id == 0)
    {
      model.SectionId = activesectionid;
    }

    return PartialView("~/Views/Section/_AddEditTableForm.cshtml", model);

  }

  // Get AddEdit section Form Partial View

  public async Task<IActionResult> GetAddEditSectionForm(int id = 0)
  {

    var model = await _sectionservice.GetSectionDetailById(id);

    return PartialView("~/Views/Section/_AddEditSectionForm.cshtml", model);

  }

  [HttpPost]
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanAddEdit)]
  public async Task<IActionResult> AddEditTableForm(AddTableViewmodel model)
  {
    AuthResponse response;
    if (model.TableId != 0)
    {
      response = await _sectionservice.EditTable(model);
    }
    else
    {
      response = await _sectionservice.AddTable(model);
    }


    return Json(new { message = response.Message, success = response.Success });

  }
  [HttpPost]
  [AuthorizePermission(PermissionName.TableAndSection, ActionPermission.CanAddEdit)]
  public async Task<IActionResult> AddEditSectionForm(SectionNameViewModel model)
  {
    AuthResponse response;
    if (model.SectionId != 0)
    {
      response = await _sectionservice.EditSection(model);
    }
    else
    {
      response = await _sectionservice.AddSection(model);
    }


    return Json(new { message = response.Message, success = response.Success });

  }

  public async Task<int> GetBusyTableCountOfSection(int sectionid = 0)
  {
    return await _sectionservice.GetBusyTableCountOfSection(sectionid);
  }
}
