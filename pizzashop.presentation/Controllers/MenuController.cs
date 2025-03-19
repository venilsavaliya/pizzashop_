using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
            Category = new AddCategoryViewModel(),
            Menuitem = new AddItemViewModel(),
            ModifierGroup = new AddModifierGroupViewModel(),
            ModifierItem = new AddModifierItemViewModel()
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
    // GET : all Modifier Item List {Partial View Return}

    public IActionResult GetAllModifierItemsListForAdd(int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        var modifiersmodel = _menuservices.GetAllModifierItemsList(pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/Menu/_ModifieritemListForAdd.cshtml", modifiersmodel);
    }

    // POST : Menu

    [HttpPost]
    public IActionResult Category(MenuViewModel model)
    {
        if (model.Category.Id != null)
        {

            var res = _menuservices.EditCategory(model.Category);

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
            var AuthResponse = _menuservices.AddCategory(model.Category);
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


   

    // GET : ModifierName List By Ids

    public IActionResult GetModifierNameListFromIds(List<string> modifierIds)
    {
        List<string> names = _menuservices.GetModifierNamesByIds(modifierIds);

        return Json(names);
    }

    #region Category

    // POST : Edit Category
    [HttpPost]
    public IActionResult EditCategoryById(MenuViewModel model)
    {
        var AuthResponse = _menuservices.EditCategory(model.Category);

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
    public IActionResult AddNewItem(MenuViewModel model)
    {
        string modifiersJson = Request.Form["ModifierGroups"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersJson))
        {
            model.Menuitem.ModifierGroups = JsonConvert.DeserializeObject<List<ModifierGroup>>(modifiersJson);
        }
        var AuthResponse = _menuservices.AddNewItem(model.Menuitem).Result;



        if (!AuthResponse.Success)
        {
            _notyf.Error(AuthResponse.Message);
            return RedirectToAction("Menu", "Menu");
        }

        TempData["ToastrType"] = "success";  // Options: success, error, warning, info
        TempData["ToastrMessage"] = "item add Successfully!";
        // return RedirectToAction("Menu","Menu");
        string categoryName = _menuservices.GetCategoryNameFromId((int)model.Menuitem.CategoryId);
        return Json(new { redirectTo = Url.Action("Index", "Menu", new { cat = categoryName }) });

    }

    // Post : Edit item
    [HttpPost]
    public IActionResult EditItem(MenuViewModel model)
    {
        string modifiersJson = Request.Form["ModifierGroups"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersJson))
        {
            model.Menuitem.ModifierGroups = JsonConvert.DeserializeObject<List<ModifierGroup>>(modifiersJson);
        }

        var AuthResponse = _menuservices.EditItem(model.Menuitem).Result;

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
            return RedirectToAction("Index", "Menu");
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        string categoryName = _menuservices.GetCategoryNameFromId((int)model.Menuitem.CategoryId);
        return Json(new { redirectTo = Url.Action("Index", "Menu", new { cat = categoryName }) });

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
    // pwd

    //  Return Partial view Of Modifier Item list based on modifier group id
    // public async Task<IActionResult> GetModifierItemsNamePVByModifierGroupid(int modifiergroup_id)
    // {
    //     try
    //     {
    //         // Get list of modifiers
    //         var modifiers = _menuservices.GetModifierItemListNamesByModifierGroupId(modifiergroup_id);

    //         // Extract modifier IDs
    //         var modifierIds = modifiers.Select(m => m.ModifiergroupId).ToList();

    //         return PartialView("~/Views/Menu/_ModifierName.cshtml",modifiers);
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { message = "Error fetching modifiers", error = ex.Message });
    //     }
    // }

    public async Task<IActionResult> GetModifierItemsNamePVByModifieritemId(int modifier_id)
    {
        try
        {
            // Get list of modifiers
            var modifiers = _menuservices.GetModifierItemNamesByModifierItemId(modifier_id);

            return PartialView("~/Views/Menu/_ModifieritemName.cshtml",modifiers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching modifiers", error = ex.Message });
        }
    }
    // Return List Of Modifier Item id list based on modifier group id
    public IActionResult GetModifierItemsidByModifierGroupid(int modifiergroup_id)
    {
        try
        {
            // Get list of modifiers
            var modifiers = _menuservices.GetModifierItemListNamesByModifierGroupId(modifiergroup_id);

            // Extract modifier IDs
            var modifierIds = modifiers.Select(m => m.ModifiergroupId).ToList();

            return Json(modifierIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching modifiers", error = ex.Message });
        }
    }

    // Return partial view of modifier group name by modifier group id
    public IActionResult GetModifierGroupNamePVByModifierGroupid(int modifiergroup_id)
    {
        try
        {
            var model = _menuservices.GetModifierGroupNamePVByModifierGroupid(modifiergroup_id);
            return PartialView("~/Views/Menu/_ModifierName.cshtml", model);
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
    public IActionResult GetModifierItemsForEdit(int modifierGroupId, int itemid)
    {
        var modifierItems = _menuservices.GetModifierItemswithMinMaxByGroupIdandItemid(modifierGroupId, itemid); // Fetch Data

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", modifierItems);
    }


    public IActionResult GetItemModifierGroupminMaxMapping(int itemid, int modifiergroupid)
    {
        var model = _menuservices.GetItemModifierGroupminMaxMappingAsync(itemid, modifiergroupid);

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", model);
    }

    public List<int> GetModifierGroupIdsByItemId(int itemid)
    {
        return _menuservices.GetModifierGroupIdsByItemId(itemid);
    }

    [HttpPost]
    public IActionResult EditModifierGroup(MenuViewModel model)
    {   
        string modifiersitemsJson = Request.Form["ModifierItems"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierGroup.ModifierItems = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }
        var response = _menuservices.EditModifierGroup(model.ModifierGroup).Result;

        if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

        return Json(new { redirectTo = Url.Action("Index", "Menu") });
       
    }
    [HttpPost]
    public IActionResult AddModifierGroup(MenuViewModel model)
    {   
        string modifiersitemsJson = Request.Form["ModifierItems"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierGroup.ModifierItems = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }
        var response = _menuservices.AddModifierGroup(model.ModifierGroup).Result;

        if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

        return Json(new { redirectTo = Url.Action("Index", "Menu") });
       
    }

    //  Delete Modifier Group by Id
    public IActionResult DeleteModifierGroupById(string id)
    {   
        
        var response = _menuservices.DeleteModifierGroupById(id).Result;

        if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

        return RedirectToAction("Index","Menu");
       
    }

    [HttpPost]
    public IActionResult AddModifierItem(MenuViewModel model)
    {   
        
        var response = _menuservices.AddModifierItem(model.ModifierItem).Result;

        if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

        return RedirectToAction("Index","Menu");
       
    }
    [HttpPost]
    public IActionResult EditModifierItem(MenuViewModel model)
    {   
        
        var response = _menuservices.EditModifierItem(model.ModifierItem).Result;

        if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

        return RedirectToAction("Index","Menu");
       
    }

    // Delete Modifier Item
     public IActionResult DeleteModifierItemById(int modifierid,int modifiergroupid)
    {   
        
        var response = _menuservices.DeleteModifierItemById(modifierid,modifiergroupid).Result;

        if(!response.Success)
        {
        TempData["ToastrType"] = "error";
        TempData["ToastrMessage"] = response.Message;
        }
    
        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = response.Message;

        return RedirectToAction("Index","Menu");
       
    }

    // Delete Multiple Modifier Items
    [HttpPost]

    public IActionResult DeleteModifierItems(int ModifierGroupid,List<string> ids)
    {

        var AuthResponse = _menuservices.DeleteModifierItems(ModifierGroupid,ids).Result;

        // if(!AuthResponse.Success)
        // {
        //     _notyf.Error(AuthResponse.Message);
        //     return RedirectToAction("Menu");
        // }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;

        // return RedirectToAction("Menu","Menu",new {cat});

        return Json(new { redirectTo = Url.Action("Index", "Menu") });

    }

    #endregion

}
