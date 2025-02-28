using BLL.Models;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IAdminService
{
    public IEnumerable<Role> GetAllRoles();

    public RolesPermissionListViewModel GetRolespermissionsByRoleId(string Roleid);
    
    public string GetRoleNameByRoleId(string id);

    public  Task<AuthResponse> SavePermission(List<RolesAndPermissionViewModel> permissions);

    

}
