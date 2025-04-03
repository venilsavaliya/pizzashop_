using DAL.ViewModels;

namespace BLL.Interfaces;

public interface ICustomerService
{
    public Task<CustomerListPaginationViewModel> GetCustomerList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "", DateTime? startDate = null, DateTime? endDate = null);

    public  Task<IEnumerable<CustomerViewModel>> GetCustomerListForExport(string searchKeyword = "", DateTime? startDate = null, DateTime? endDate = null);

    public Task<CustomerHistoryDetailViewModel> GetCustomerOrderHistory(int orderid);
}
