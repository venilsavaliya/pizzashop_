namespace pizzashop.presentation.Controllers;
using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

public class SectionController : BaseController
{
   private readonly ISectionServices _sectionservice;
   public SectionController(IJwtService jwtService,IUserService userService,IAdminService adminservice,ISectionServices sectionservice) : base(jwtService,userService,adminservice)
    {
       _sectionservice = sectionservice;
    }

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
           Table = new AddTableViewmodel()
        };
       return View(model);
    }

  // Get : Return partial View Of Pagination Table List

  public IActionResult GetDiningTableList(int id, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
      
        var model = _sectionservice.GetDiningTablesListBySectionId(id, pageNumber, pageSize, searchKeyword);
        ViewBag.active = "Menu";

        return PartialView("~/Views/Section/_TableList.cshtml", model);
    }

    // GET : Section List {Partial View Return}

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

   // POST : Add New Section
   [HttpPost]
    public IActionResult AddSection(SectionAndTableViewModel model)
    {
      var response = _sectionservice.AddSection(model.Section).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message; 

      return RedirectToAction("Index","Section");
    }
   // POST : Edit New Section
   [HttpPost]
    public IActionResult EditSection(SectionAndTableViewModel model)
    {
      var response = _sectionservice.EditSection(model.Section).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message; 

      return RedirectToAction("Index","Section");
    }

    // Delete Section


    public IActionResult DeleteSection(int id)
    {
        var AuthResponse = _sectionservice.DeleteSection(id).Result;

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        return RedirectToAction("Index", "Section");
    }

    // POST : Add New Table
   [HttpPost]
    public IActionResult AddTable(SectionAndTableViewModel model)
    {
      if (!ModelState.IsValid)
      {   
          var sections = _sectionservice.GetSectionList().ToList();
          model.Sections = sections;
          return View("Index", model); // Re-render the view with validation errors
      }

      var response = _sectionservice.AddTable(model.Table).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

      return RedirectToAction("Index","Section");
    }
   [HttpPost]
    public IActionResult EditTable(SectionAndTableViewModel model)
    {
      var response = _sectionservice.EditTable(model.Table).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

      return RedirectToAction("Index","Section");
    }

    // Delete Table
    public IActionResult DeleteTable(int id)
    {
        var AuthResponse = _sectionservice.DeleteTable(id).Result;

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        return RedirectToAction("Index", "Section");
    }

    // POST : Delete Multiple Tables
    [HttpPost]

    public IActionResult DeleteTables(List<int> ids)
    {

      var AuthResponse = _sectionservice.DeleteTables(ids).Result;

       if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        // return RedirectToAction("Index", "Section");

        return Json(new { redirectTo = Url.Action("Index", "Section") });

    }
}
