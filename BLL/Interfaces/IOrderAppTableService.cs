using BLL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppTableService
{
    public Task<List<OrderAppTableViewModel>> GetOrderAppTableAndSectionList();

    public Task<AuthResponse> AssignTableAsync(TableAssignViewModel model);
}