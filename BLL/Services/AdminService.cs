using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


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




    // GET : List Of All Roles
    public List<Role> GetAllRoles()
    {
        try
        {
            return _context.Roles.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllRoles: {ex.Message}");

            return new List<Role>();
        }

    }

    // GET : Rolename Of User By RoleId
    public string GetRoleNameByRoleId(string id)
    {
        try
        {
            var roledata = _context.Roles.FirstOrDefault(r => r.Roleid.ToString() == id);

            return roledata?.Name ?? "Unknown";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRoleNameByRoleId: {ex.Message}");

            return "Error";
        }
    }

    // GET : Get All Permission Of Perticular Role 
    public RolesPermissionListViewModel GetRolespermissionsByRoleId(string Roleid)
    {
        try
        {
            // Fetch Role Permissions By Roleid

            List<RolesAndPermissionViewModel> rolesAndPermissions = _context.Rolespermissions
            .Where(x => x.Roleid.ToString() == Roleid)
            .Select(x => new RolesAndPermissionViewModel
            {
                Id = x.Id.ToString(),
                PermissionName = x.Permission.PermissionName,
                Canedit = x.Canedit,
                Canview = x.Canview,
                Candelete = x.Candelete,
                Isenable = x.Isenable ?? false
            })
            .OrderBy(p => p.PermissionName)
            .ToList();

            return new RolesPermissionListViewModel
            {
                Permissionlist = rolesAndPermissions.ToList(),
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRolespermissionsByRoleId: {ex.Message}");

            return new RolesPermissionListViewModel();
        }
    }

    // GET : Save Permissions Of User
    public async Task<AuthResponse> SavePermission(RolesPermissionListViewModel p)
    {
        try
        {
            /* 
            Iterate Throgh List Of Incoming Permissions
            Get Individual Permission One By One And Update It Accordingly
            */

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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SavePermission: {ex.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = $"Error in SavePermission: {ex.Message}"
            };
        }

    }

    // GET : Get Dashboard Data 
    public async Task<DashboardViewModel> GetDashboardData(DateTime startdate, DateTime enddate)
    {

        try
        {
            var totalOrder = await _context.Orders.Where(i => i.OrderDate.Date >= startdate.Date && i.OrderDate.Date <= enddate.Date && i.OrderStatus != Constants.OrderCancelled).CountAsync();

            var totalSales = await _context.Orders.Where(i => i.OrderDate.Date >= startdate.Date && i.OrderDate.Date <= enddate.Date && i.OrderStatus != Constants.OrderCancelled).SumAsync(i => i.TotalAmount);

            decimal AverageOrderValue = 0;

            if (totalOrder != 0)
            {
                AverageOrderValue = totalSales / totalOrder;
            }

            var ordersList = await _context.Dishritems
                                    .Include(i => i.Order)
                                    .Include(i => i.Item)
                                    .Where(i => i.Order.OrderDate.Date >= startdate.Date && i.Order.OrderDate.Date <= enddate.Date && i.Order.OrderStatus != Constants.OrderCancelled)
                                    .ToListAsync();


            // Group by Orderid and compute max(Averageservingtime) per order
            var groupedOrders = ordersList
                                    .GroupBy(i => i.Orderid)
                                    .ToList();

            var averageWaitingTime = groupedOrders.Any()
                ? groupedOrders.Average(g => g.Max(i => i.Averageservingtime))
                : 0;

            var WaitingListCount = await _context.Waitingtokens.Where(i => i.Completiontime == null && i.Isdeleted!=true).CountAsync();

            var NewCustomerCount = await _context.Customers.Where(i => i.Createddate.Date >= startdate.Date && i.Createddate.Date <= enddate.Date).CountAsync();


            var revenueList = await _context.Orders
                                        .Where(i => i.OrderDate.Date >= startdate.Date && i.OrderDate.Date <= enddate.Date && i.OrderStatus != Constants.OrderCancelled)
                                        .GroupBy(i => i.OrderDate.Date)
                                        .OrderBy(g => g.Key)
                                        .Select(g => new
                                        {
                                            Date = g.Key.ToString("yyyy-MM-dd"),
                                            Total = g.Sum(i => i.TotalAmount)
                                        }
                                        ).ToListAsync();

            var customerGrowthList = await _context.Customers
                                        .Where(i => i.Createddate.Date >= startdate.Date && i.Createddate.Date <= enddate.Date)
                                        .GroupBy(i => i.Createddate.Date)
                                        .OrderBy(g => g.Key)
                                        .Select(g => new

                                        { Date = g.Key.ToString("yyyy-MM-dd"), Total = g.Count() }
                                        ).ToListAsync();


            var RevenueData = new Dictionary<string, decimal>();

            var CustomerGrowthData = new Dictionary<string, int>();

            // Fill Revenue Data in The Dictionary
            foreach (var revenue in revenueList)
            {
                RevenueData[revenue.Date] = revenue.Total;
            }

            // Convert customerGrowthList (List Of Object) In To Dictionary(List Of Key Value Pair)
            var customerGrowthDict = customerGrowthList.ToDictionary(x => x.Date, x => x.Total);

            // Fill Customer Data in The Dictionary
            var customerTotal = 0;

            for (DateTime date = startdate.Date; date <= enddate.Date; date = date.AddDays(1))
            {
                var currdate = date.ToString("yyyy-MM-dd");
                // RevenueData[currdate] = 0;

                if (customerGrowthDict.ContainsKey(currdate))
                {
                    customerTotal += customerGrowthDict[currdate];
                }
                CustomerGrowthData[currdate] = customerTotal;

                if(!RevenueData.ContainsKey(currdate))
                {
                    RevenueData[currdate]=0;
                }
            }



            // Fill Dates For Show In Labels In Charts
            var dateList = new List<string>();

            for (var date = startdate; date <= enddate; date = date.AddDays(1))
            {
                dateList.Add(date.ToString("yyyy-MM-dd"));
            }

            // List Of Selling Item Order By Total Order Of That Item
            var sellingItemLists = (from d in _context.Dishritems
                                    join i in _context.Items on d.Itemid equals i.ItemId into joined
                                    from i in joined.DefaultIfEmpty()
                                    group d by new { d.Itemid, i.ItemName, i.Image } into g
                                    select new SellingItemList
                                    {
                                        ItemId = g.Key.Itemid,
                                        ItemName = g.Key.ItemName,
                                        Image = g.Key.Image,
                                        TotalOrder = g.Where(i => i.Order.OrderDate.Date >= startdate.Date && i.Order.OrderDate.Date <= enddate.Date && i.Order.OrderStatus != Constants.OrderCancelled).Select(x => x.Orderid).Distinct().Count()
                                    })
                                    .OrderByDescending(x => x.TotalOrder)
                                    .ToList();


            return new DashboardViewModel
            {
                TotalOrders = totalOrder,
                AverageOrderValue = AverageOrderValue,
                TotalSales = totalSales,
                AverageWaitingTime = (decimal)averageWaitingTime,
                WaitingListCount = WaitingListCount,
                CustomerGrowthCount = CustomerGrowthData,
                Dates = dateList,
                NewCustomerCount = NewCustomerCount,
                RevenueList = RevenueData,
                SellingItems = sellingItemLists,
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDashboardData: {ex.Message}");

            return new DashboardViewModel();
        }

    }

}
