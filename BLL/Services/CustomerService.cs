namespace BLL.Services;
using BLL.Helper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContext;

    private readonly IJwtService _jwtservices;

    private readonly IUserService _userservices;

    public CustomerService(ApplicationDbContext dbcontext, IHttpContextAccessor httpContext, IUserService userService, IJwtService jwtservices)
    {
        _context = dbcontext;
        _httpContext = httpContext;
        _userservices = userService;
        _jwtservices = jwtservices;
    }

    // Get Paginated Customer List
    public async Task<CustomerListPaginationViewModel> GetCustomerList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "", DateTime? startDate = null, DateTime? endDate = null)
    {
        searchKeyword = searchKeyword.ToLower();
        CustomerListPaginationViewModel model = new() { Page = new() };

        var query = from u in _context.Customers
                    select new CustomerViewModel
                    {
                        CustomerId = u.CustomerId,
                        Name = u.Name,
                        Email = u.Email,
                        Mobile = u.Mobile,
                        JoinDate = u.Createddate,
                        TotalVisit = u.TotalVisit
                    };



        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u =>
                                 u.Name.ToLower().Contains(searchKeyword) ||
                                 u.Email.ToLower().Contains(searchKeyword)
                                );
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(u => u.JoinDate >= startDate.Value.Date && u.JoinDate.Date <= endDate.Value.Date);
        }

        // Sorting logic
        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
        {
            switch (sortColumn)
            {
                case "Name":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name);
                    break;
                case "Date":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.JoinDate) : query.OrderByDescending(u => u.JoinDate);
                    break;
                case "TotalAmount":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.TotalVisit) : query.OrderByDescending(u => u.TotalVisit);
                    break;
                default:
                    query = query.OrderBy(u => u.CustomerId); // **Ensure a default sorting**
                    break;
            }
        }
        else
        {
            query = query.OrderBy(u => u.CustomerId); // **Apply a default ordering if no sort is provided**
        }


        // Pagination
        int totalCount = query.Count();
        var customerList = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = customerList;
        model.SortColumn = sortColumn;
        model.SortOrder = sortOrder;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
        return model;
    }

    // Get Paginated Customer List
    public async Task<IEnumerable<CustomerViewModel>> GetCustomerListForExport(string searchKeyword = "", DateTime? startDate = null, DateTime? endDate = null)
    {
        searchKeyword = searchKeyword.ToLower();


        var query = from u in _context.Customers
                    select new CustomerViewModel
                    {
                        CustomerId = u.CustomerId,
                        Name = u.Name,
                        Email = u.Email,
                        Mobile = u.Mobile,
                        JoinDate = u.Createddate,
                        TotalVisit = u.TotalVisit
                    };



        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u =>
                                 u.Name.ToLower().Contains(searchKeyword) ||
                                 u.Email.ToLower().Contains(searchKeyword)
                                );
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(u => u.JoinDate >= startDate.Value.Date && u.JoinDate.Date <= endDate.Value.Date);
        }



        query = query.OrderBy(u => u.CustomerId); // **Apply a default ordering if no sort is provided**



        // Pagination
        int totalCount = query.Count();
        var customerList = query.ToList();

        return customerList;
    }

    // Get Customer Order History 

    public async Task<IEnumerable<CustomerHistoryViewModel>> GetCustomerOrderHistory(int customerid)
    {
        var query = from o in _context.Orders
                    where o.CustomerId == customerid
                    select new CustomerHistoryViewModel
                    {
                        Date = o.OrderDate,
                        OrderType = true,
                        PaymentType = o.PaymentMode,
                        Totalitems = _context.Dishritems.Where(i=>i.Orderid == o.OrderId).Count(),
                        TotalAmount = o.TotalAmount
                    };

        return query.ToList();
    }

}
