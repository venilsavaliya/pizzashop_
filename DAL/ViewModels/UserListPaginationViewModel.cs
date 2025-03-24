namespace DAL.ViewModels;

public class UserListPaginationViewModel
{
    public IEnumerable<UserViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }

    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
}
