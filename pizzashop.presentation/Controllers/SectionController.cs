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
           Sections = sections
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
    public IActionResult AddSection(AddSectionViewModel model)
    {
      var response = _sectionservice.AddSection(model).Result;

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
    public IActionResult EditSection(AddSectionViewModel model)
    {
      var response = _sectionservice.EditSection(model).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message; 

      return RedirectToAction("Index","Section");
    }

    // POST : Add New Table
   [HttpPost]
    public IActionResult AddTable(AddTableViewmodel model)
    {
      var response = _sectionservice.AddTable(model).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

      return RedirectToAction("Index","Section");
    }
}
