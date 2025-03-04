using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BLL.Services;

public class AdminService : IAdminService
{

    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IEmailService _emailService;

    private readonly IWebHostEnvironment _env;
    public AdminService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, IEmailService emailService, IWebHostEnvironment env)
    {

        _context = context;

        _httpContextAccessor = contextAccessor;

        _emailService = emailService;

        _env = env;
    }


    public IEnumerable<Role> GetAllRoles()
    {

        return _context.Roles.ToList();

    }

    public string GetRoleNameByRoleId(string id)
    {
        var roledata = _context.Roles.FirstOrDefault(r => r.Roleid.ToString() == id);
        return roledata.Name;
    }

    public RolesPermissionListViewModel GetRolespermissionsByRoleId(string Roleid)
    {
        // Fetch role permissions by Roleid
        var rolesAndPermissions = _context.Rolespermissions
            .Where(x => x.Roleid.ToString() == Roleid)
            .Select(x => new RolesAndPermissionViewModel
            {
                // RoleName = GetRoleNameByRoleId(x.Roleid.ToString()), 
                Id = x.Id.ToString(),
                PermissionName = x.Permission.PermissionName,
                Canedit = x.Canedit,
                Canview = x.Canview,
                Candelete = x.Candelete,
                Isenable = x.Isenable ?? false
            })
            .ToList();

        return new RolesPermissionListViewModel{
            Permissionlist = rolesAndPermissions.ToList(),
        };
    }

    public async Task<AuthResponse> SavePermission(RolesPermissionListViewModel p)
    {   
        foreach (var permission in p.Permissionlist)
        {
            var existingPermission = _context.Rolespermissions.FirstOrDefault(p => p.Id.ToString() == permission.Id.ToString());

            if (existingPermission != null)
            {
                existingPermission.Isenable = permission.Isenable;
                existingPermission.Canview = permission.Canview;
                existingPermission.Canedit = permission.Canedit;
                existingPermission.Candelete = permission.Candelete;
            }
        }

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Permission Changes Successfully"
        };
    }

}
