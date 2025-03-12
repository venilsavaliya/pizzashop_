using DAL.Models;

namespace DAL.ViewModels;

public class MenuViewModel
{
    public int? SelectedCategory {get;set;}

    public int? SelectedModifierGroup {get;set;}

    public IEnumerable<ModifierGroupNameViewModel> ModifierGroups {get;set;}

    public IEnumerable<CategoryNameViewModel> Categories {get;set;}

    // public IEnumerable<ItemViewModel> Items {get;set;}
    public ItemPaginationViewModel Itemsmodel {get;set;}
}
