using BLL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppMenuService
{

    public Task<List<OrderAppMenuItemViewModel>> GetMenuItem(int catid = 0, string searchkeyword = "", bool isfav = false);

    public Task<OrderAppModifierItemList> GetModifierGroupsByItemId(int itemId);

    public Task<AuthResponse> ChangeStatusOfFavouriteItem(int itemid);

    public Task<MenuItemModifierGroupMappiingViewModel> GetModifierItemsOfMenuItem(int id);

    public Task<OrderAppMenuMainViewModel> GetOrderDetailByOrderId(int orderid);

    public Task<AuthResponse> SaveOrderAsync(SaveOrderItemsViewModel model);

    public int GetReadyQuantityOfItem(int id);

    public Task<OrderCustomerDetailViewModel> GetCustomerDetailsByOrderId(int orderid);

    public Task<AuthResponse> SaveCustomerDetail(OrderCustomerDetailViewModel model);

    public Task<AuthResponse> SaveOrderInstruction(InstructionViewModel model);

    public Task<InstructionViewModel> GetInstruction(int dishid = 0, int orderid = 0, int index = 0, string Instruction = "");

    public Task<AuthResponse> CompleteOrder(SaveOrderItemsViewModel model);

    public string GetOrderStatus(int orderid);

    public Task<AuthResponse> CancelOrder(int orderid);
    
    public  Task<AuthResponse> OrderReview(OrderReviewViewModel model);
}
