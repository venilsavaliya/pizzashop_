namespace DAL.ViewModels;

public class MenuItemsPaginationViewModel
{
    public int CategoryId {get;set;}
    public IEnumerable<ItemViewModel>? Items { get; set; } = new List<ItemViewModel>();
    public Pagination? Page { get; set; } = new Pagination();
} 
 