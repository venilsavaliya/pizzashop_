using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class MenuController : Controller
{

    private readonly IMenuServices _menuservices;

    private readonly INotyfService _notyf;

    public MenuController(IMenuServices menuServices, INotyfService notyf)
    {
        _menuservices = menuServices;
        _notyf = notyf;
    }

    // GET : Menu

    public IActionResult Menu()
    {
        var categories = _menuservices.GetCategoryList();

        return View(categories);
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
}
