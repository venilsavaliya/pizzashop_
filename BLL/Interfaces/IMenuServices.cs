using BLL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IMenuServices
{


    public IEnumerable<CategoryNameViewModel> GetCategoryList();
    public AuthResponse AddCategory(AddCategoryViewModel model);
    public AuthResponse EditCategory(AddCategoryViewModel model);
    public AuthResponse DeleteCategory(string id);
    public ItemPaginationViewModel GetItemsListByCategoryName(string category,int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public Task<AuthResponse> AddNewItem(AddItemViewModel model);
    public Task<AuthResponse> EditItem(AddItemViewModel model);

    public Task<AuthResponse> DeleteItems(List<string> ids);

    public string GetCategoryNameFromId(int id);

    public IEnumerable<ModifierGroupNameViewModel> GetModifiersGroupList();
    public ModifierItemsPagination GetModifierItemsListByModifierGroupId(int modifiergroup_id, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public ModifierItemModalPagination GetAllModifierItemsList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public Task<AuthResponse> AddNewModifierGroup(AddModifierGroupViewModel model);

    public List<string> GetModifierNamesByIds(List<string> modifierIds);
}
