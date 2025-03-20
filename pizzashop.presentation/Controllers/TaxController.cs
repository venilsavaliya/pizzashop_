using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class TaxController:BaseController
{

    private readonly ITaxService _taxservice;

    public TaxController(IJwtService jwtService,IUserService userService,IAdminService adminservice,ITaxService taxservice):base(jwtService,userService,adminservice)
    {
        _taxservice = taxservice;
    }

    public IActionResult Index()
    {  
        ViewBag.active = "Tax";
        return View();
    }

    // Get Partial View OF TaxTableList

    public IActionResult GetTaxList(int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
      
        var model = _taxservice.GetTaxList( pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/Tax/_TableList.cshtml", model);
    }

    // POST : Add Tax
   [HttpPost]
    public IActionResult AddTax(MainTaxViewModel model)
    {
      var response = _taxservice.AddTax(model.Taxes).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message; 

      return RedirectToAction("Index","Tax");
    }
    // POST : Edit Tax
   [HttpPost]
    public IActionResult EditTax(MainTaxViewModel model)
    {
      var response = _taxservice.EditTax(model.Taxes).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message; 

      return RedirectToAction("Index","Tax");
    }
    // Delete Tax

    public IActionResult DeleteTax(int id)
    {
      var response = _taxservice.DeleteTax(id).Result;

       if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message; 

      return RedirectToAction("Index","Tax");
    }
}
