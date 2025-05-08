namespace DAL.ViewModels;

public class DashboardViewModel
{

    public int TotalOrders {get;set;}

    public decimal AverageOrderValue {get;set;}

    public decimal TotalSales {get;set;}

    public decimal AverageWaitingTime {get;set;}

    public int WaitingListCount {get;set;}

    public int NewCustomerCount {get;set;}

    public List<string> Dates {get;set;}


    public Dictionary<string,decimal> RevenueList {get;set;} = new Dictionary<string, decimal>();
    public Dictionary<string,int> CustomerGrowthCount {get;set;} = new Dictionary<string, int>();

    public List<SellingItemList> SellingItems {get;set;} = new List<SellingItemList>();
}

public class SellingItemList
{
    public string ItemName {get;set;}

    public int ItemId {get;set;}

    public int TotalOrder {get;set;}

    public string Image {get;set;}
}
