using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class MenuController : BaseController
{

    private readonly IMenuServices _menuservices;

    private readonly INotyfService _notyf;

    public MenuController(IMenuServices menuServices, INotyfService notyf,IJwtService jwtService,IUserService userService,IAdminService adminservice) : base(jwtService,userService,adminservice)
    {
        _menuservices = menuServices;
        _notyf = notyf;
    }

    // GET : Menu

    public IActionResult Menu(string? cat, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")

    {
        var categories = _menuservices.GetCategoryList();
        if(cat==null){
            cat = categories.First().Name;
        }
        var itempaginationmodel = _menuservices.GetItemsListByCategoryName(cat,  pageNumber , pageSize , searchKeyword); 
        ViewBag.active = "Menu";

        // var itempaginationmodel = new ItemPaginationViewModel{
        //     Items = items,
        //     TotalCount
        // };

        var model = new MenuViewModel{
            Categories = categories,
            Itemsmodel = itempaginationmodel,
            SelectedCategory = cat
        };

       
        return View(model);
    }

    // POST : Menu

    [HttpPost]
    public IActionResult Menu(AddCategoryViewModel model)
    {
        if (model.Id != null)
        {

            var res = _menuservices.EditCategory(model);

            if (res.Success)
            {
                _notyf.Success(res.Message);
            }

        }
        else
        {
            var AuthResponse = _menuservices.AddCategory(model);
            if (AuthResponse.Success)
                _notyf.Success(AuthResponse.Message);
        }

        TempData["ToastrType"] = "success";  // Options: success, error, warning, info
        TempData["ToastrMessage"] = "category add Successfully!";

        return RedirectToAction("Menu", "Menu");
    }

    // POST : Delete category

    public IActionResult DeleteCategory(string id)
    {
        var AuthResponse = _menuservices.DeleteCategory(id);
        return RedirectToAction("Menu","Menu");
    }

    // POST : ADD New Item

    public IActionResult AddNewItem(AddItemViewModel model){

        var AuthResponse = _menuservices.AddNewItem(model).Result;

        if(!AuthResponse.Success)
        {
            _notyf.Error(AuthResponse.Message);
            return RedirectToAction("Menu","Menu");
        }

        TempData["ToastrType"] = "success";  // Options: success, error, warning, info
        TempData["ToastrMessage"] = "item add Successfully!";
        // return RedirectToAction("Menu","Menu");
        string categoryName = _menuservices.GetCategoryNameFromId((int)model.CategoryId);
        return Json(new { redirectTo = Url.Action("Menu", "Menu",new {cat=categoryName}) });
        
    }

    // Post : Edit item
    [HttpPost]
    public IActionResult EditItem(AddItemViewModel model){

        var AuthResponse = _menuservices.EditItem(model).Result;

        if(!AuthResponse.Success)
        {
            _notyf.Error(AuthResponse.Message);
            return RedirectToAction("Menu","Menu");
        }

        TempData["ToastrType"] = "success";  
        TempData["ToastrMessage"] = AuthResponse.Message;
        string categoryName = _menuservices.GetCategoryNameFromId((int)model.CategoryId);
        return RedirectToAction("Menu","Menu",new {cat=categoryName});

        // return Json(new { redirectTo = Url.Action("Menu", "Menu",new {cat=categoryName}) });
        
    }

    // Post : Delete Items
    [HttpPost]

     public IActionResult DeleteItems(string cat,List<string> ids){

        var AuthResponse = _menuservices.DeleteItems(ids).Result;

        // if(!AuthResponse.Success)
        // {
        //     _notyf.Error(AuthResponse.Message);
        //     return RedirectToAction("Menu");
        // }

        TempData["ToastrType"] = "success";  
        TempData["ToastrMessage"] = AuthResponse.Message;

        // return RedirectToAction("Menu","Menu",new {cat});

        return Json(new { redirectTo = Url.Action("Menu", "Menu") });
        
    }



}
