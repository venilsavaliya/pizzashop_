using BLL.Models;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IWaitingTokenService
{
    public TaxListPaginationViewModel GetTaxList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "");

    // public Task<AuthResponse> AddWaitingTokenAsync(AddEditWaitingTokenViewModel model);

    public  Task<AuthResponse> EditTax(AddTaxViewModel model);

    public Task<AuthResponse> DeleteTax(int id);

}
