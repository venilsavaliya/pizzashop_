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
                        TotalVisit = _context.Orders.Count(i=>i.CustomerId==u.CustomerId)
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
                case "TotalOrder":
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
    public async Task<List<CustomerViewModel>> GetCustomerListForExport(string searchKeyword = "", DateTime? startDate = null, DateTime? endDate = null)
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

    public async Task<CustomerHistoryDetailViewModel> GetCustomerOrderHistory(int customerid)
    {
        var customer = await _context.Customers
            .Where(c => c.CustomerId == customerid)
            .Select(c => new CustomerHistoryDetailViewModel
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Mobile = c.Mobile,
                MaxOrderAmount = _context.Orders
                    .Where(i => i.CustomerId == c.CustomerId)
                    .Max(i => (decimal?)i.TotalAmount) ?? 0, // Safe for empty results
                AverageOrderAmount = _context.Orders
                    .Where(i => i.CustomerId == c.CustomerId)
                    .Average(i => (decimal?)i.TotalAmount) ?? 0,
                JoinDate = c.Createddate,
                // Leave Items null for now
            })
            .FirstOrDefaultAsync();

        if (customer == null) return null;

        customer.Items = await _context.Orders
            .Where(o => o.CustomerId == customerid)
            .Select(i => new CustomerHistoryViewModel
            {
                Date = i.OrderDate,
                PaymentType = i.PaymentMode,
                OrderType = i.Ordertype == 1,
                TotalAmount = i.TotalAmount,
                Totalitems = _context.Dishritems.Count(j => j.Orderid == i.OrderId)
            })
            .ToListAsync();

        customer.TotalVisit = customer.Items.Count();

        return customer;
    }

    // Add New Customer

    public async Task<int> AddCustomer(AddCustomerViewModel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            // check if customer already exists

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == model.Email);

            if (customer != null)
            {
                customer.Name = model.Name;
                customer.TotalVisit += 1;
                customer.Mobile = model.Mobile;
                customer.Totalperson = model.TotalPerson;
                await _context.SaveChangesAsync();
                return customer.CustomerId;
            }
            else
            {
                var newCustomer = new Customer
                {
                    Name = model.Name,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Totalperson = model.TotalPerson,
                    Createdby = userid,
                    Createddate = DateTime.Now,
                    TotalVisit = 1
                };

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                if (newCustomer.CustomerId != 0)
                {
                    return newCustomer.CustomerId;
                }
                else
                {
                    return 0;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in Add Token: {e.Message}");

            return 0;
        }
    }

    // Get Detail Of Customer From Email

    public async Task<CustomerViewModel> GetCustomerDetail(string email)
    {
        try
        {
            if (email != null)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

                if (customer != null)
                {
                    return new CustomerViewModel
                    {
                        Name = customer.Name,
                        Email = customer.Email,
                        Mobile = customer.Mobile
                    };
                }
                else
                {
                    return new CustomerViewModel();
                }
            }

            return new CustomerViewModel();
        }
        catch (Exception)
        {
            return new CustomerViewModel();
        }
    }

}
