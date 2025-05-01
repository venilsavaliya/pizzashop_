using BLL.Models;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IAdminService
{
    public IEnumerable<Role> GetAllRoles();

    public RolesPermissionListViewModel GetRolespermissionsByRoleId(string Roleid);
    
    public string GetRoleNameByRoleId(string id);

    public  Task<AuthResponse> SavePermission(RolesPermissionListViewModel p);

    public Task<DashboardViewModel> GetDashboardData(DateTime startdate, DateTime enddate);

    
}
