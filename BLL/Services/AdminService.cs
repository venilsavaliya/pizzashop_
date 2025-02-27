using BLL.Interfaces;
using DAL.Models;
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


    public IEnumerable<Role> GetAllRoles(){
        
        return _context.Roles.ToList();

    }

    public IEnumerable<Rolespermission> GetRolespermissionsByRoleId(string Roleid)
    {   
        return _context.Rolespermissions.Where(x => x.Roleid.ToString() == Roleid).ToList();
    }
    
}
