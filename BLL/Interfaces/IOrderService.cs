using DAL.ViewModels;
namespace BLL.Interfaces;

public interface IOrderService
{
    public Task<OrderListPaginationViewModel> GetOrderList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "",string status="",DateTime? startDate = null, DateTime? endDate = null);

     public Task<IEnumerable<OrderViewModel>> GetOrderListForExport(string searchKeyword = "", string status = "", DateTime? startDate = null, DateTime? endDate = null);
}
