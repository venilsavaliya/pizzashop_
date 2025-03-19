using DAL.Models;

namespace DAL.ViewModels;

public class MenuViewModel 
{
    public int? SelectedCategory {get;set;}

    public int? SelectedModifierGroup {get;set;}

    public IEnumerable<ModifierGroupNameViewModel> ModifierGroups {get;set;}

    public IEnumerable<CategoryNameViewModel> Categories {get;set;}

    public AddItemViewModel Menuitem {get;set;} = new AddItemViewModel();

    public AddCategoryViewModel Category {get;set;} = new AddCategoryViewModel();

    public AddModifierGroupViewModel ModifierGroup {get;set;} = new AddModifierGroupViewModel();

    public AddModifierItemViewModel ModifierItem {get;set;} = new AddModifierItemViewModel();

    // public IEnumerable<ItemViewModel> Items {get;set;}
    public ItemPaginationViewModel Itemsmodel {get;set;}
}
