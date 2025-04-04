using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppKOTService
{
    public Task<List<CategoryNameViewModel>> GetCategoriesAsync();

    public Task<OrderDetailKOTViewModel> GetOrderDetailsAsync(int categoryId, bool quantityType = false);
}
