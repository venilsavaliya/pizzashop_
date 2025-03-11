namespace DAL.ViewModels;

public class ModifierItemsPagination
{
    public IEnumerable<ModifierItemsViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }
}
