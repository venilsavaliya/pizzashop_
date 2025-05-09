namespace DAL.ViewModels;

public class ModifierItemsPagination
{   
    public int ModifierGroupId {get;set;}
    public IEnumerable<ModifierItemsViewModel>? Items { get; set; } = new List<ModifierItemsViewModel>();
    public Pagination? Page { get; set; } = new Pagination();
}
