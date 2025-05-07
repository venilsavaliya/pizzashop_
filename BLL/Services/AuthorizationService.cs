namespace BLL.Services;
using BLL.Interfaces;
using DAL.Constants;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


public class AuthorizationService : IAuthorizationService
{
    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContext;

    private readonly IUserService _userservices;

    public AuthorizationService(ApplicationDbContext context,IHttpContextAccessor httpContext,IUserService userservices)
    {
        _context = context;
        _httpContext = httpContext;
        _userservices = userservices;
    }

    public bool HasPermission(string module, ActionPermission action)
    {
    
        var token = _httpContext.HttpContext?.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        if (userid == null)
            return false;

        var roleid = _context.Userdetails.FirstOrDefault(u => u.UserId == userid).RoleId;

        var permissionid = _context.Permissions.FirstOrDefault(p=> p.PermissionName.ToLower() == module.ToLower()).PermissionId;
   

        // var permission = user.Role.Permissions.FirstOrDefault(p => p.ModuleName == module);

        var permission = _context.Rolespermissions.FirstOrDefault(p=> p.Roleid == roleid && p.PermissionId == permissionid );

        if (permission == null)
            return false;

        return action switch
        {
            ActionPermission.CanAddEdit => permission.Canedit,
            ActionPermission.CanView => permission.Canview,
            ActionPermission.CanDelete => permission.Candelete,
            _ => false
        };
    }
}
