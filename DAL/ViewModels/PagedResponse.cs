namespace DAL.ViewModels;

public class PagedResponse<T>
{
    public List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int? TotalCount { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int? TotalPages { get; set; }
    public int? StartIndex { get; set; }
    public int? EndIndex { get; set; }
    public string? SearchKeyword { get; set; }

    public PagedResponse(List<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
