namespace DAL.ViewModels;



public class CustomerHistoryViewModel
{
    public DateTime Date {get;set;}

    public bool OrderType {get;set;}

    public string PaymentType {get;set;}

    public int Totalitems {get;set;}

    public decimal TotalAmount {get;set;}

}
