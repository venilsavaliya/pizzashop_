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

        return RedirectToAction("Menu", "Menu");
    }

    // POST : Delete category

    public IActionResult DeleteCategory(string id)
    {
        var AuthResponse = _menuservices.DeleteCategory(id);
        return RedirectToAction("Menu","Menu");
    }

    #region Items



    #endregion
}
