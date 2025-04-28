namespace DAL.ViewModels;

public class OrderAppMenuMainViewModel 
{
    public int OrderId {get;set;}

    public int CustomerId {get;set;}

    public int SectionId {get;set;}

    public string SectionName {get;set;} = "Unknown";

    public string OrderComment {get;set;}

    public List<TableCapacityList> TableList {get;set;} = new List<TableCapacityList>();

    public List<MenuOrderItemViewModel> OrderItems = new List<MenuOrderItemViewModel>();

    public List<TaxViewModel> TaxList = new List<TaxViewModel>();

}

public class TableCapacityList{

    public int TableId {get;set;}

    public string Name {get;set;}

    public int Capacity {get;set;}

}
