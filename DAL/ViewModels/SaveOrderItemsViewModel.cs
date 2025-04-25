namespace DAL.ViewModels;

public class SaveOrderItemsViewModel
{
    public int OrderId {get;set;}

    public string OrderInstruction {get;set;}="";

    public List<MenuOrderItemViewModel> OrderItems = new List<MenuOrderItemViewModel>();
    public string? OrderItemData {get;set;}


}
