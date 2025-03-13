using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace pizzashop.presentation.Controllers;

public class MenuController : BaseController
{

    private readonly IMenuServices _menuservices;

    private readonly INotyfService _notyf;

    public MenuController(IMenuServices menuServices, INotyfService notyf, IJwtService jwtService, IUserService userService, IAdminService adminservice) : base(jwtService, userService, adminservice)
    {
        _menuservices = menuServices;
        _notyf = notyf;
    }
    // GET : Index

    public IActionResult Index(int? cat, int? ModifierId)
    {

        var categories = _menuservices.GetCategoryList();
        var ModifierGroups = _menuservices.GetModifiersGroupList();

        if (cat == null)
        {
            cat = categories.First().Id;
        }
        if (ModifierId == null)
        {
            ModifierId = ModifierGroups.First().ModifiergroupId;
        }
        // var itempaginationmodel = _menuservices.GetItemsListByCategoryName(cat, pageNumber, pageSize, searchKeyword);
        ViewBag.active = "Menu";

        var model = new MenuViewModel
        {
            Categories = categories,
            SelectedCategory = cat,
            SelectedModifierGroup = ModifierId,
            ModifierGroups = ModifierGroups,
            // Itemsmodel = itempaginationmodel
        };

        return View(model);

    }
    // GET : Menu {Returns Partial View}

    public IActionResult Menu(int cat, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
        var categories = _menuservices.GetCategoryList();

        // var defaultcat = categories.First().Id;
        if (cat == null)
        {
            cat = categories.First().Id;
        }

        var itempaginationmodel = _menuservices.GetItemsListByCategoryId(cat, pageNumber, pageSize, searchKeyword);
        ViewBag.active = "Menu";

        // var model = new MenuViewModel
        // {
        //     Categories = categories,
        //     Itemsmodel = itempaginationmodel,
        //     SelectedCategory = cat
        // };

        return PartialView("~/Views/Menu/_MenuItemsList.cshtml", itempaginationmodel);
    }


    // GET : Category List {Partial View Return}

    public IActionResult GetCategories(int? cat)
    {
        var categories = _menuservices.GetCategoryList().ToList();

        if (cat == null)
        {
            cat = categories.First().Id;
        }

        var model = new CategoryListViewModel
        {
            Categories = categories,
            SelectedCategory = cat
        };

        return PartialView("~/Views/Menu/_CategoryList.cshtml", model);
    }

    // GET : Modifier List {Partial View Return}

    public IActionResult GetModifiers(int? modifiergroup_id)
    {
        var modifiers = _menuservices.GetModifiersGroupList().ToList();

        if (modifiergroup_id == null)
        {
            modifiergroup_id = modifiers.First().ModifiergroupId;
        }

        var model = new ModifierListViewModel
        {
            ModifierGroups = modifiers,
            SelectedModifierGroup = modifiergroup_id
        };

        return PartialView("~/Views/Menu/_ModifierList.cshtml", model);
    }
    // GET : Modifier Item List {Partial View Return}

    public IActionResult GetModifierItemsList(int modifiergroup_id, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {

        // var modifierlist = _menuservices.GetModifiersGroupList();
        // if (modifiergroup_id == null)
        // {
        //     modifiergroup_id = modifierlist.First().ModifiergroupId;
        // }

        var modifiersmodel = _menuservices.GetModifierItemsListByModifierGroupId(modifiergroup_id, pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/Menu/_ModifierItemsList.cshtml", modifiersmodel);
    }
    // GET : all Modifier Item List {Partial View Return}

    public IActionResult GetAllModifierItemsList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        var modifiersmodel = _menuservices.GetAllModifierItemsList(pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/Menu/_ModifierItemsListModal.cshtml", modifiersmodel);
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
                TempData["ToastrType"] = "success";
                TempData["ToastrMessage"] = res.Message;
            }
            else
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = res.Message;
            }

        }
        else
        {
            var AuthResponse = _menuservices.AddCategory(model);
            if (AuthResponse.Success)
            {
                TempData["ToastrType"] = "success";
                TempData["ToastrMessage"] = AuthResponse.Message;
            }
            else
            {
                TempData["ToastrType"] = "error";
                TempData["ToastrMessage"] = "error in add category";
            }

        }

        return RedirectToAction("Index", "Menu");
    }


    // POST : ADD ModifierGroup 
    [HttpPost]
    public IActionResult AddModifierGroup(AddModifierGroupViewModel model)
    {

        var AuthResponse = _menuservices.AddNewModifierGroup(model).Result;
        // if (model.Id != null)
        // {

        //     var res = _menuservices.EditCategory(model);

        //     if (res.Success)
        //     {
        //         TempData["ToastrType"] = "success";
        //         TempData["ToastrMessage"] = res.Message;
        //     }

        // }
        // else
        // {
        //     var AuthResponse = _menuservices.AddCategory(model);
        //     if (AuthResponse.Success)
        //     {
        //         TempData["ToastrType"] = "success";
        //         TempData["ToastrMessage"] = AuthResponse.Message;
        //     }

        // }

        // TempData["ToastrType"] = "error";
        // TempData["ToastrMessage"] = "error in add category";

        return RedirectToAction("Index", "Menu");
    }

    // GET : ModifierName List By Ids

    public IActionResult GetModifierNameListFromIds(List<string> modifierIds)
    {
        List<string> names = _menuservices.GetModifierNamesByIds(modifierIds);

        return Json(names);
    }

    #region Category

    // POST : Edit Category
    [HttpPost]
    public IActionResult EditCategoryById(AddCategoryViewModel model)
    {
        var AuthResponse = _menuservices.EditCategory(model);

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        return RedirectToAction("Index", "Menu");
    }


    // POST : Delete category

    public IActionResult DeleteCategory(string id)
    {
        var AuthResponse = _menuservices.DeleteCategory(id).Result;

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        return RedirectToAction("Index", "Menu");
    }

    #endregion

    #region Menu items

    // POST : ADD New Item
    [HttpPost]
    public IActionResult AddNewItem(AddItemViewModel model)
    {
        string modifiersJson = Request.Form["ModifierGroups"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersJson))
        {
            model.ModifierGroups = JsonConvert.DeserializeObject<List<ModifierGroup>>(modifiersJson);
        }
        var AuthResponse = _menuservices.AddNewItem(model).Result;



        if (!AuthResponse.Success)
        {
            _notyf.Error(AuthResponse.Message);
            return RedirectToAction("Menu", "Menu");
        }

        TempData["ToastrType"] = "success";  // Options: success, error, warning, info
        TempData["ToastrMessage"] = "item add Successfully!";
        // return RedirectToAction("Menu","Menu");
        string categoryName = _menuservices.GetCategoryNameFromId((int)model.CategoryId);
        return Json(new { redirectTo = Url.Action("Index", "Menu", new { cat = categoryName }) });

    }

    // Post : Edit item
    [HttpPost]
    public IActionResult EditItem(AddItemViewModel model)
    {

        var AuthResponse = _menuservices.EditItem(model).Result;

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
            return RedirectToAction("Index", "Menu");
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        string categoryName = _menuservices.GetCategoryNameFromId((int)model.CategoryId);
        return RedirectToAction("Index", "Menu", new { cat = categoryName });

        // return Json(new { redirectTo = Url.Action("Menu", "Menu",new {cat=categoryName}) });

    }

    // Post : Delete Items
    [HttpPost]

    public IActionResult DeleteItems(List<string> ids)
    {

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

    // Post : Delete Items
    // [HttpPost]
    public IActionResult DeleteSingleItem(int id, int catid)
    {

        var AuthResponse = _menuservices.DeleteSingleItem(id).Result;

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
            return RedirectToAction("Menu");
        }

        else
        {
            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = AuthResponse.Message;
            // return Json(new { redirectTo = Url.Action("Menu", "Menu") });
            return RedirectToAction("Index", "Menu", new { cat = catid });
        }

    }

    #endregion

    #region Modifier 

    // Return List Of Modifier Item list based on modifier group id
    public IActionResult GetModifierItemsNameByModifierGroupid(int modifiergroup_id)
    {
        try
        {
            var modifiers = _menuservices.GetModifierItemListNamesByModifierGroupId(modifiergroup_id);
            return Ok(modifiers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching modifiers", error = ex.Message });
        }
    }

    // Return Partial View for modifier items in add and update menu items modal
    public IActionResult GetModifierItems(int modifierGroupId)
    {
        var modifierItems = _menuservices.GetModifierItemsByGroupId(modifierGroupId); // Fetch Data

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", modifierItems);
    }
    public IActionResult GetModifierItemsForEdit(int modifierGroupId)
    {
        var modifierItems = _menuservices.GetModifierItemsByGroupId(modifierGroupId); // Fetch Data

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", modifierItems);
    }


    public IActionResult GetItemModifierGroupminMaxMapping(int itemid , int modifiergroupid)
    {
        var model = _menuservices.GetItemModifierGroupminMaxMappingAsync(itemid,modifiergroupid);

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", model);
    }

    #endregion

}
