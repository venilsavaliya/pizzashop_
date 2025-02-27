using DAL.Models;

namespace BLL.Interfaces;

public interface IAdminService
{
    public IEnumerable<Role> GetAllRoles();

    public IEnumerable<Rolespermission> GetRolespermissionsByRoleId(string Roleid);

    

}
