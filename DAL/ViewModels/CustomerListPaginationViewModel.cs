namespace DAL.ViewModels;

public class CustomerListPaginationViewModel
{
    public IEnumerable<CustomerViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
}
