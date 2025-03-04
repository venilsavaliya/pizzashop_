namespace DAL.ViewModels;

public class ItemPaginationViewModel
{
    public IEnumerable<ItemViewModel> Items { get; set; }  // List of users
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int StartIndex { get; set; } 
    public int EndIndex { get; set; }
    public string? SearchKeyword { get; set; } 
}
