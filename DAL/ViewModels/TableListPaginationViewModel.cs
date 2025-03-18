namespace DAL.ViewModels;

public class TableListPaginationViewModel
{
    public int Sectionid {get;set;}
    public IEnumerable<TableViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }
}

