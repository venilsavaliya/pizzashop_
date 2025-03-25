namespace DAL.ViewModels;

public class OrderListPaginationViewModel
{
    public IEnumerable<OrderViewModel>? Items { get; set; }

    public Pagination? Page { get; set; }

    public string? SortColumn { get; set; }

    public string? SortOrder { get; set; }
}
