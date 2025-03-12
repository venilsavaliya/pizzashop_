namespace DAL.ViewModels;

public class MenuItemsPaginationViewModel
{
    public int CategoryId {get;set;}
    public IEnumerable<ItemViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }
}
 