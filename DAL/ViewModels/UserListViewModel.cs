namespace DAL.ViewModels;

public class UserListViewModel
{
    public List<UserViewModel> Users { get; set; } = new List<UserViewModel>(); // List of users
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public string? SearchKeyword { get; set; }
}

