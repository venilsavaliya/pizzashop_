using BLL.Models;
using DAL.ViewModels;
using DAL.Models;
namespace BLL.Interfaces;

public interface IMenuServices
{
    public List<Unit> GetAllUnitsList();

    public AddModifierItemViewModel GetModifierItemDetailById(int id);

    public IEnumerable<CategoryNameViewModel> GetCategoryList();

    public CategoryNameViewModel GetCategoryDetailById(int id);

    public Task<AuthResponse> AddCategory(CategoryNameViewModel model);

    public Task<AuthResponse> EditCategory(CategoryNameViewModel model);

    public Task<AuthResponse> DeleteCategory(string id);

    public MenuItemsPaginationViewModel GetItemsListByCategoryId(int categoryid, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");

    public Task<ModifierGroupanditemsViewModel> GetItemModifierGroupminMaxMappingAsync(int itemId, int modifierGroupId);

    public AddItemViewModel GetMenuItemDetailById(int id);

    public Task<AuthResponse> AddNewItem(AddItemViewModel model);

    public Task<AuthResponse> EditItem(AddItemViewModel model);

    public Task<AuthResponse> DeleteSingleItem(int id);

    public Task<AuthResponse> DeleteItems(List<string> ids);

    public IEnumerable<ModifierGroupNameViewModel> GetModifiersGroupList();

    public AddModifierGroupViewModel GetModifierGroupDetailById(int id);

    public List<int> GetModifierGroupIdsByItemId(int itemId);

    public Task<List<object>> GetModifierGroupIdListByModifierItemId(int modifierItemId);

    public ModifierGroupNameViewModel GetModifierGroupNamePVByModifierGroupid(int modifiergroup_id);

    public List<string> GetModifierNamesByIds(List<string> modifierIds);

    public Task<AuthResponse> AddModifierGroup(AddModifierGroupViewModel model);

    public Task<AuthResponse> EditModifierGroup(AddModifierGroupViewModel model);

    public Task<AuthResponse> DeleteModifierGroupById(string id);

    public ModifierItemsPagination GetModifierItemsListByModifierGroupId(int modifiergroup_id, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");

    public ModifierItemModalPagination GetAllModifierItemsList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "");

    public List<ModifierGroupNameViewModel> GetModifierItemListNamesByModifierGroupId(int modifiergroup_id);

    public ModifierGroupanditemsViewModel GetModifierItemsByGroupId(int modifiergroup_id);

    public ModifierGroupanditemsViewModel GetModifierItemswithMinMaxByGroupIdandItemid(int modifiergroup_id, int itemid);

    public ModifierItemNameViewModel GetModifierItemNamesByModifierItemId(int modifieritem_id);

    public Task<AuthResponse> AddModifierItem(AddModifierItemViewModel model);

    public Task<AuthResponse> EditModifierItem(AddModifierItemViewModel model);

    public Task<AuthResponse> DeleteModifierItemById(int modifierid, int modifiergroupid);

    public Task<AuthResponse> DeleteModifierItems(int ModifierGroupid, List<string> ids);

}
