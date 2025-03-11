namespace DAL.ViewModels;

public class ModifierItemModalPagination
{
    public IEnumerable<ModifierItemsViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }
}
