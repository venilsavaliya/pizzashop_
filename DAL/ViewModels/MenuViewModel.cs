using DAL.Models;

namespace DAL.ViewModels;

public class MenuViewModel
{

    public string? SelectedCategory {get;set;}
    public IEnumerable<CategoryNameViewModel> Categories {get;set;}

    // public IEnumerable<ItemViewModel> Items {get;set;}
    public ItemPaginationViewModel Itemsmodel {get;set;}
}
