namespace DAL.ViewModels;

public class SaveOrderItemsViewModel
{
    public int OrderId {get;set;}

    public string OrderInstruction {get;set;}="";

    public List<MenuOrderItemViewModel> OrderItems = new List<MenuOrderItemViewModel>();

    public List<TaxViewModel> TaxList = new List<TaxViewModel>();

    public string? OrderItemData {get;set;}

    public decimal TotalAmount {get;set;} = 0;

}
