namespace DAL.ViewModels;

public class ExportCustomerFilterViewModel
{
    public string SearchKeyword { get; set; } = string.Empty;
    public string StartDate { get; set; } = null!;
    public string EndDate { get; set; } = null!;
    public string Timeframe { get; set; } = string.Empty;
}
