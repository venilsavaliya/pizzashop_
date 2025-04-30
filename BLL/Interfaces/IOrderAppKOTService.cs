using BLL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppKOTService
{
    public Task<List<CategoryNameViewModel>> GetCategoriesAsync();

    public Task<List<OrderDetailKOTViewModel>> GetOrderDetailsAsync(int categoryId, bool isPending = false);

    public Task<List<OrderDishKOTViewModel>> GetOrderitemListAsync(int orderid,bool isPending = false);

    public  Task<AuthResponse> UpdateOrderQuantityAsync(List<OrderItemQuantityViewModel> items,bool MarkasPrepared = true);

    public Task<AuthResponse> UpdateServedQuantityAsync(List<DishItemServeQuantityViewModel> items);
}
