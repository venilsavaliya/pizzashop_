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


    public List<Dictionary<string,decimal>> RevenueList {get;set;}
    public List<Dictionary<string,int>> CustomerGrowthCount {get;set;}

    public List<SellingItemList> SellingItems {get;set;}
}

public class SellingItemList
{
    public string ItemName {get;set;}

    public int TotalQuantity {get;set;}

    public string image {get;set;}
}
