using BLL.Models;
using DAL.ViewModels;
using DAL.Models;
namespace BLL.Interfaces;

public interface IMenuServices
{


    public IEnumerable<CategoryNameViewModel> GetCategoryList();
    public Task<AuthResponse> AddCategory(AddCategoryViewModel model);
    public Task<AuthResponse> EditCategory(AddCategoryViewModel model);
    public Task<AuthResponse> DeleteCategory(string id);
    public Task<AuthResponse> DeleteSingleItem(int id);
    public MenuItemsPaginationViewModel GetItemsListByCategoryId(int categoryid,int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public Task<AuthResponse> AddNewItem(AddItemViewModel model);
    public Task<AuthResponse> EditItem(AddItemViewModel model);

    public Task<AuthResponse> DeleteItems(List<string> ids);

    public string GetCategoryNameFromId(int id);

    public IEnumerable<ModifierGroupNameViewModel> GetModifiersGroupList();

    public Task<List<object>> GetModifierGroupIdListByModifierItemId(int modifierItemId);
    public ModifierItemsPagination GetModifierItemsListByModifierGroupId(int modifiergroup_id, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public ModifierItemModalPagination GetAllModifierItemsList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public Task<AuthResponse> AddModifierGroup(AddModifierGroupViewModel model);

    public List<string> GetModifierNamesByIds(List<string> modifierIds);

    public List<ModifierGroupNameViewModel> GetModifierItemListNamesByModifierGroupId(int modifiergroup_id);

    public ModifierGroupanditemsViewModel GetModifierItemsByGroupId(int modifiergroup_id);

    public ModifierGroupanditemsViewModel GetModifierItemswithMinMaxByGroupIdandItemid(int modifiergroup_id,int itemid);

    public Task<ModifierGroupanditemsViewModel> GetItemModifierGroupminMaxMappingAsync(int itemId, int modifierGroupId);

    public List<int> GetModifierGroupIdsByItemId(int itemId);

    public ModifierGroupNameViewModel GetModifierGroupNamePVByModifierGroupid(int modifiergroup_id);

   public ModifierItemNameViewModel GetModifierItemNamesByModifierItemId(int modifieritem_id);

   public Task<AuthResponse> EditModifierGroup(AddModifierGroupViewModel model);

   public Task<AuthResponse> DeleteModifierGroupById(string id);
   public Task<AuthResponse> AddModifierItem (AddModifierItemViewModel model);

   public Task<AuthResponse> EditModifierItem (AddModifierItemViewModel model);
   public Task<AuthResponse> DeleteModifierItemById(int modifierid,int modifiergroupid);

   public  Task<AuthResponse> DeleteModifierItems(int ModifierGroupid,List<string> ids);

   
}
