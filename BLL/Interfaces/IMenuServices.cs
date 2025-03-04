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
}
