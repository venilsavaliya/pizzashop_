using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using BLL.Services;
using DAL.Constants;
using BLL.Attributes;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BLL.Models;

namespace pizzashop.presentation.Controllers;

public class MenuController : BaseController
{

    private readonly IMenuServices _menuservices;


    private readonly INotyfService _notyf;

    public MenuController(IMenuServices menuServices, INotyfService notyf, IJwtService jwtService, IUserService userService, IAdminService adminservice, IAuthorizationService authservice) : base(jwtService, userService, adminservice, authservice)
    {
        _menuservices = menuServices;
        _notyf = notyf;

    }
    // GET : Index

    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
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

    // Get AddEdit Category Form Partial View
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public IActionResult GetAddEditCategoryForm(int id = 0)
    {
        if (id == 0)
        {
            return PartialView("~/Views/Menu/_CategoryAddEditForm.cshtml", new CategoryNameViewModel());
        }
        else
        {
            var model = _menuservices.GetCategoryDetailById(id);
            return PartialView("~/Views/Menu/_CategoryAddEditForm.cshtml", model);
        }
    }
    // Get AddEdit Modifier Group Form Partial View
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public IActionResult GetAddEditModifierGroupForm(int id = 0)
    {
        if (id == 0)
        {
            return PartialView("~/Views/Menu/_ModifierGroupAddEditForm.cshtml", new AddModifierGroupViewModel());
        }
        else
        {
            var model = _menuservices.GetModifierGroupDetailById(id);
            return PartialView("~/Views/Menu/_ModifierGroupAddEditForm.cshtml", model);
        }
    }

    // Get AddEdit Modifier Item Form Partial View
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public IActionResult GetAddEditModifierItemForm(int id = 0)
    {

        var model = _menuservices.GetModifierItemDetailById(id);
        return PartialView("~/Views/Menu/_AddEditModifierItemForm.cshtml", model);

    }

    // Get AddEdit Menuitem Form Partial View
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public IActionResult GetAddEditMenuItemForm(int id = 0)
    {
        if (id == 0)
        {
            var model = new AddItemViewModel();
            model.ModifierGroupNames = _menuservices.GetModifiersGroupList().ToList();
            model.Categories = _menuservices.GetCategoryList().ToList();
            model.UnitsList = _menuservices.GetAllUnitsList();

            return PartialView("~/Views/Menu/_MenuitemAddEditForm.cshtml", model);
        }
        else
        {
            var model = _menuservices.GetMenuItemDetailById(id);
            return PartialView("~/Views/Menu/_MenuitemAddEditForm.cshtml", model);
        }
    }

    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddEditMenuItem(AddItemViewModel model)
    {
        string modifiersJson = Request.Form["ModifierGroups"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersJson))
        {
            model.ModifierGroups = JsonConvert.DeserializeObject<List<ModifierGroup>>(modifiersJson);
        }

        if (model.Id == 0)
        {
            var response = await _menuservices.AddNewItem(model);
            return Json(new { message = response.Message, success = response.Success });
        }
        else
        {
            var response = await _menuservices.EditItem(model);
            return Json(new { message = response.Message, success = response.Success });
        }

    }

    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddEditCategory(CategoryNameViewModel model)
    {
        if (model.Id == 0)
        {
            var response = await _menuservices.AddCategory(model);
            return Json(new { message = response.Message, success = response.Success });
        }
        else
        {
            var response = await _menuservices.EditCategory(model);
            return Json(new { message = response.Message, success = response.Success });
        }

    }

    // Get : Modifier Group List Return Json

    public IActionResult GetModifierGroupListData()
    {
        var ModifierGroups = _menuservices.GetModifiersGroupList();
        return Json(ModifierGroups);
    }

    // Get : Category List Return Json

    public IActionResult GetCategoryListData()
    {
        var categories = _menuservices.GetCategoryList();

        return Json(categories);
    }

    // GET : Menu {Returns Partial View}
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult Menu(int cat, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
        var categories = _menuservices.GetCategoryList();

        // var defaultcat = categories.First().Id;
        if (cat == 0)
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
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetCategories(int? cat)
    {
        var categories = _menuservices.GetCategoryList().ToList();

        if (cat == 0 || cat == null)
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
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetModifiers(int? modifiergroup_id)
    {
        var modifiers = _menuservices.GetModifiersGroupList().ToList();

        if (modifiergroup_id == 0 || modifiergroup_id == null)
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
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetModifierItemsList(int modifiergroup_id, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {

        var modifierlist = _menuservices.GetModifiersGroupList();
        if (modifiergroup_id == 0)
        {
            modifiergroup_id = modifierlist.First().ModifiergroupId;
        }

        var modifiersmodel = _menuservices.GetModifierItemsListByModifierGroupId(modifiergroup_id, pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/Menu/_ModifierItemsList.cshtml", modifiersmodel);
    }
    // GET : all Modifier Item List {Partial View Return}
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetAllModifierItemsList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        var modifiersmodel = _menuservices.GetAllModifierItemsList(pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/Menu/_ModifierItemsListModal.cshtml", modifiersmodel);
    }
    // GET : all Modifier Item List {Partial View Return}
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetAllModifierItemsListForAdd(int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        var modifiersmodel = _menuservices.GetAllModifierItemsList(pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/Menu/_ModifieritemListForAdd.cshtml", modifiersmodel);
    }

    // // POST : Menu
    // [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    // [HttpPost]
    // public IActionResult Category(MenuViewModel model)
    // {
    //     if (model.Category.Id != null)
    //     {

    //         var res = _menuservices.EditCategory(model.Category).Result;

    //         return Json(new { Success = res.Success, message = res.Message });
    //     }
    //     else
    //     {
    //         var AuthResponse = _menuservices.AddCategory(model.Category).Result;
    //         return Json(new { Success = AuthResponse.Success, message = AuthResponse.Message });
    //     }

    // }




    // GET : ModifierName List By Ids
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetModifierNameListFromIds(List<string> modifierIds)
    {
        List<string> names = _menuservices.GetModifierNamesByIds(modifierIds);

        return Json(names);
    }

    #region Category

    // // POST : Edit Category
    // [HttpPost]
    // [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    // public IActionResult EditCategoryById(MenuViewModel model)
    // {
    //     var AuthResponse = _menuservices.EditCategory(model.Category).Result;

    //     if (!AuthResponse.Success)
    //     {
    //         TempData["ToastrType"] = "error";
    //         TempData["ToastrMessage"] = AuthResponse.Message;

    //         return Json(new { Success = false, message = AuthResponse.Message });
    //     }
    //     else
    //     {
    //         TempData["ToastrType"] = "success";
    //         TempData["ToastrMessage"] = AuthResponse.Message;
    //         return Json(new { Success = true, message = AuthResponse.Message });
    //     }
    // }


    // POST : Delete category 
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    [HttpPost]
    public IActionResult DeleteCategory(string id)
    {
        var AuthResponse = _menuservices.DeleteCategory(id).Result;

        if (!AuthResponse.Success)
        {
            return Json(new { Success = false, message = AuthResponse.Message });
        }
        else
        {
            return Json(new { Success = true, message = AuthResponse.Message });
        }



    }

    #endregion

    #region Menu items 

    // POST : ADD New Item
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
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
            return Json(new { success = false, message = AuthResponse.Message });
        }
        else
        {
            return Json(new { success = true, message = AuthResponse.Message });
        }

        // TempData["ToastrType"] = "success";
        // TempData["ToastrMessage"] = "item add Successfully!";
        // // return RedirectToAction("Menu","Menu");
        // string categoryName = _menuservices.GetCategoryNameFromId((int)model.Menuitem.CategoryId);
        // return Json(new { redirectTo = Url.Action("Index", "Menu", new { cat = categoryName }) });

    }

    // Post : Edit item
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
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
            return Json(new { Success = false, message = AuthResponse.Message });
        }
        else
        {
            return Json(new { Success = true, message = AuthResponse.Message });
        }


        // string categoryName = _menuservices.GetCategoryNameFromId((int)model.Menuitem.CategoryId);
        // return Json(new { redirectTo = Url.Action("Index", "Menu", new { cat = categoryName }) });

        // return Json(new { redirectTo = Url.Action("Menu", "Menu",new {cat=categoryName}) });

    }

    // Post : Delete Items
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    public IActionResult DeleteItems(List<string> ids)
    {

        var AuthResponse = _menuservices.DeleteItems(ids).Result;

        if (!AuthResponse.Success)
        {
            return Json(new { success = false, message = AuthResponse.Message });
        }
        else
        {
            return Json(new { success = true, message = AuthResponse.Message });
        }



        // return RedirectToAction("Menu","Menu",new {cat});



    }

    // Post : Delete Items
    // [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    [HttpPost]
    public IActionResult DeleteSingleItem(int id, int catid)
    {

        var AuthResponse = _menuservices.DeleteSingleItem(id).Result;

        if (!AuthResponse.Success)
        {
            return Json(new { success = false, message = AuthResponse.Message });
        }

        else
        {
            return Json(new { success = true, message = AuthResponse.Message });
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
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public async Task<IActionResult> GetModifierItemsNamePVByModifieritemId(int modifier_id)
    {
        try
        {
            // Get list of modifiers
            var modifiers = _menuservices.GetModifierItemNamesByModifierItemId(modifier_id);

            return PartialView("~/Views/Menu/_ModifieritemName.cshtml", modifiers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching modifiers", error = ex.Message });
        }
    }
    // Return List Of Modifier Item id list based on modifier group id
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
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
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
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
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetModifierItems(int modifierGroupId)
    {
        var modifierItems = _menuservices.GetModifierItemsByGroupId(modifierGroupId); // Fetch Data

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", modifierItems);
    }
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetModifierItemsForEdit(int modifierGroupId, int itemid)
    {
        var modifierItems = _menuservices.GetModifierItemswithMinMaxByGroupIdandItemid(modifierGroupId, itemid); // Fetch Data

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", modifierItems);
    }

    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public IActionResult GetItemModifierGroupminMaxMapping(int itemid, int modifiergroupid)
    {
        var model = _menuservices.GetItemModifierGroupminMaxMappingAsync(itemid, modifiergroupid);

        return PartialView("~/Views/Menu/_ModifierGroupanditems.cshtml", model);
    }

    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanView)]
    public List<int> GetModifierGroupIdsByItemId(int itemid)
    {
        return _menuservices.GetModifierGroupIdsByItemId(itemid);
    }

    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public IActionResult EditModifierGroup(MenuViewModel model)
    {
        string modifiersitemsJson = Request.Form["ModifierItems"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierGroup.ModifierItems = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }
        var response = _menuservices.EditModifierGroup(model.ModifierGroup).Result;

        if (!response.Success)
        {
            return Json(new { success = response.Success, message = response.Message });
        }
        else
        {
            return Json(new { success = response.Success, message = response.Message });
        }

    }
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public async Task<IActionResult> AddEditModifierGroup(AddModifierGroupViewModel model)
    {
        string modifiersitemsJson = Request.Form["ModifierItems"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierItems = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }
        AuthResponse response;
        if (model.ModifierId != null)
        {
            response = await _menuservices.EditModifierGroup(model);
        }
        else
        {
            response = await _menuservices.AddModifierGroup(model);
        }
        return Json(new { success = response.Success, message = response.Message });


    }
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public IActionResult AddModifierGroup(MenuViewModel model)
    {
        string modifiersitemsJson = Request.Form["ModifierItems"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierGroup.ModifierItems = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }
        var response = _menuservices.AddModifierGroup(model.ModifierGroup).Result;

        if (!response.Success)
        {
            return Json(new { success = response.Success, message = response.Message });
        }
        else
        {
            return Json(new { success = response.Success, message = response.Message });
        }

    }

    //  Delete Modifier Group by Id

    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    [HttpPost]
    public IActionResult DeleteModifierGroupById(string id)
    {

        var response = _menuservices.DeleteModifierGroupById(id).Result;

        return Json(new { success = response.Success, message = response.Message });

    }

    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanAddEdit)]
    public IActionResult AddModifierItem(MenuViewModel model)
    {
        string modifiersitemsJson = Request.Form["ModifierGroupid"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierItem.ModifierGroupid = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }

        var response = _menuservices.AddModifierItem(model.ModifierItem).Result;

        return Json(new { success = response.Success, message = response.Message });


    }
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    public IActionResult EditModifierItem(MenuViewModel model)
    {
        string modifiersitemsJson = Request.Form["ModifierGroupid"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierItem.ModifierGroupid = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }

        var response = _menuservices.EditModifierItem(model.ModifierItem).Result;

        return Json(new { success = response.Success, message = response.Message });
    }
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    public async Task<IActionResult> AddEditModifierItem(AddModifierItemViewModel model)
    {
        string modifiersitemsJson = Request.Form["ModifierGroupid"];

        // deserialize the modifiersjson 
        if (!string.IsNullOrEmpty(modifiersitemsJson))
        {
            model.ModifierGroupid = JsonConvert.DeserializeObject<List<int>>(modifiersitemsJson);
        }
        
        AuthResponse response;
        if (model.ModifierId != 0)
        {
            response = await _menuservices.EditModifierItem(model);
        }
        else
        {
            response = await _menuservices.AddModifierItem(model);
        }

        return Json(new { success = response.Success, message = response.Message });
    }

    public IActionResult GetModifierGroupIdListByModifierItemId(int id)
    {
        var modgrouplist = _menuservices.GetModifierGroupIdListByModifierItemId(id).Result;

        return Json(modgrouplist);
    }

    // Delete Modifier Item
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    [HttpPost]
    public IActionResult DeleteModifierItemById(int modifierid, int modifiergroupid)
    {

        var response = _menuservices.DeleteModifierItemById(modifierid, modifiergroupid).Result;

        return Json(new { success = response.Success, message = response.Message });

    }

    // Delete Multiple Modifier Items
    [HttpPost]
    [AuthorizePermission(PermissionName.Menu, ActionPermission.CanDelete)]
    public IActionResult DeleteModifierItems(int ModifierGroupid, List<string> ids)
    {

        var AuthResponse = _menuservices.DeleteModifierItems(ModifierGroupid, ids).Result;

        if (!AuthResponse.Success)
        {
            return Json(new { success = AuthResponse.Success, message = AuthResponse.Message });
        }
        else
        {
            return Json(new { success = AuthResponse.Success, message = AuthResponse.Message });
        }
    }

    #endregion

}
