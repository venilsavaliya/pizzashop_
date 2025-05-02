using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
            .OrderBy(p => p.PermissionName)
            .ToList();

        return new RolesPermissionListViewModel
        {
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

    public async Task<DashboardViewModel> GetDashboardData(DateTime startdate, DateTime enddate)
    {

        try
        {
            var totalOrder = await _context.Orders.Where(i => i.OrderDate.Date >= startdate.Date && i.OrderDate.Date <= enddate.Date).CountAsync();

            var totalSales = await _context.Orders.Where(i => i.OrderDate.Date >= startdate.Date && i.OrderDate.Date <= enddate.Date).SumAsync(i => i.TotalAmount);

            decimal AverageOrderValue = 0;
            if (totalOrder != 0)
            {
                AverageOrderValue = totalSales / totalOrder;
            }


            // var orders = _context.Dishritems.Include(i => i.Order).Include(i => i.Item);

            var ordersList = await _context.Dishritems
    .Include(i => i.Order)
    .Include(i => i.Item)
    .Where(i => i.Order.OrderDate.Date >= startdate.Date && i.Order.OrderDate.Date <= enddate.Date)
    .ToListAsync();


            // Group by Orderid and compute max(Averageservingtime) per order
            var groupedOrders = ordersList
         .GroupBy(i => i.Orderid)
         .ToList();

            var averageWaitingTime = groupedOrders.Any()
                ? groupedOrders.Average(g => g.Max(i => i.Averageservingtime))
                : 0;

            // var AverageWaitingTime = await orders.Where(i => i.Order.OrderDate >= startdate && i.Order.OrderDate <= enddate)
            // .GroupBy(i => i.Orderid).AverageAsync(i => i.Max(j => j.Averageservingtime));

            var WaitingListCount = await _context.Waitingtokens.Where(i => i.Completiontime == null).CountAsync();

            var NewCustomerCount = await _context.Customers.Where(i => i.Createddate.Date >= startdate.Date && i.Createddate.Date <= enddate.Date).CountAsync();

            // var RevenueList = await _context.Orders
            //     .Where(i => i.OrderDate >= startdate && i.OrderDate <= enddate).
            //     GroupBy(i => i.OrderDate.Date)
            //     .OrderBy(g => g.Key)
            //     .Select(i => i.Sum(j => j.TotalAmount)).ToListAsync();

            var revenueList = await _context.Orders
    .Where(i => i.OrderDate.Date >= startdate.Date && i.OrderDate.Date <= enddate.Date)
    .GroupBy(i => i.OrderDate.Date)
    .OrderBy(g => g.Key)
    .Select(g => new
    {
        Date = g.Key.ToString("yyyy-MM-dd"),
        Total = g.Sum(i => i.TotalAmount)
    }
    ).ToListAsync();


            // var CustomerGrowthCount = await _context.Customers
            //     .Where(i => i.Createddate >= startdate && i.Createddate <= enddate)
            //     .GroupBy(i => i.Createddate.Date)
            //     .OrderBy(g => g.Key)
            //     .Select(i => i.Count()).ToListAsync();

            var customerGrowthList = await _context.Customers
    .Where(i => i.Createddate.Date >= startdate.Date && i.Createddate.Date <= enddate.Date)
    .GroupBy(i => i.Createddate.Date)
    .OrderBy(g => g.Key)
    .Select(g => new

    { Date = g.Key.ToString("yyyy-MM-dd"), Total = g.Count() }
    ).ToListAsync();


            var RevenueData = new Dictionary<string, decimal>();
            var CustomerGrowthData = new Dictionary<string, int>();

            var customerGrowthDict = customerGrowthList.ToDictionary(x=> x.Date,x=> x.Total);

            // Step 1: Fill dictionary with dates and initial value 0
             var customerTotal =0;
            for (DateTime date = startdate.Date; date <= enddate.Date; date = date.AddDays(1))
            {   
                var currdate = date.ToString("yyyy-MM-dd");
                RevenueData[currdate] = 0;

                if(customerGrowthDict.ContainsKey(currdate))
                {
                    customerTotal += customerGrowthDict[currdate];
                }
                CustomerGrowthData[currdate] = customerTotal;
            }

            foreach (var revenue in revenueList)
            {
                RevenueData[revenue.Date] = revenue.Total;
            }
            
            // var customerTotal =0;
            // foreach(var customer in customerGrowthList)
            // {
            //     // customerTotal += customer.Total;
            //     CustomerGrowthData[customer.Date]=customerTotal;
            // }

            

            
            

            var dateList = new List<string>();

            for (var date = startdate; date <= enddate; date = date.AddDays(1))
            {
                dateList.Add(date.ToString("yyyy-MM-dd"));
            }

            // List<SellingItemList> sellingItemLists = ordersList.Select(i => new SellingItemList
            // {
            //     ItemName = i.Item.ItemName,
            //     TotalQuantity = (int)ordersList.Where(j => j.Order.OrderDate >= startdate && j.Order.OrderDate <= enddate && j.Itemid == i.Itemid).Sum(i => i.Quantity),
            //     image = i.Item.Image
            // }).ToList();

            //   var sellingitems = await _context.Dishritems
            //                       .GroupBy(i=>i.Itemid).Distinct(i=>i.Orderid);

            //        var sellingItemLists = await _context.Dishritems
            // .Include(i => i.Order)
            // .Include(i => i.Item)
            // .Where(i => i.Order.OrderDate >= startdate && i.Order.OrderDate <= enddate)
            // .GroupBy(i=>i.Orderid)
            // .Select(i=> new SellingItemList
            // {
            //     ItemName = 
            // })
            // .ToListAsync();

            var sellingItemLists = (from d in _context.Dishritems
             join i in _context.Items on d.Itemid equals i.ItemId into joined
             from i in joined.DefaultIfEmpty()
             group d by new { d.Itemid, i.ItemName, i.Image } into g
             select new SellingItemList
             {
                 ItemId = g.Key.Itemid,
                 ItemName = g.Key.ItemName,
                 Image = g.Key.Image,
                 TotalOrder = g.Where(i=>i.Order.OrderDate.Date>=startdate.Date && i.Order.OrderDate.Date<=enddate.Date).Select(x => x.Orderid).Distinct().Count()
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
        catch (System.Exception)
        {
            throw;
        }

    }

}
