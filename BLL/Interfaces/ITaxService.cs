using BLL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface ITaxService
{
    public TaxListPaginationViewModel GetTaxList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "");

    public TaxViewModel GetTaxDetailById(int id);

    public Task<AuthResponse> AddTax(TaxViewModel model);

    public  Task<AuthResponse> EditTax(TaxViewModel model);

    public Task<AuthResponse> DeleteTax(int id);

}
