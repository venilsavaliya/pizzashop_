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

    public async Task<CustomerHistoryDetailViewModel> GetCustomerOrderHistory(int customerid)
    {

        var query = from o in _context.Orders
                    where o.CustomerId == customerid
                    select new CustomerHistoryDetailViewModel
                    {
                        CustomerId = o.CustomerId,
                        Name = o.Customer.Name,
                        Mobile = o.Customer.Mobile,
                        MaxOrderAmount = _context.Orders.Where(i => i.CustomerId == customerid).Max(i => i.TotalAmount),
                        AverageOrderAmount = _context.Orders.Where(i => i.CustomerId == customerid).Average(i => i.TotalAmount),
                        TotalVisit = _context.Customers.FirstOrDefault(i => i.CustomerId == customerid).TotalVisit,
                        JoinDate = o.Customer.Createddate,
                        Items = (from od in _context.Orders
                                 where od.CustomerId == customerid
                                 select new CustomerHistoryViewModel
                                 {
                                     Date = od.OrderDate,
                                     OrderType = true,
                                     PaymentType = od.PaymentMode,
                                     Totalitems = _context.Dishritems.Where(i => i.Orderid == od.OrderId).Count(),
                                     TotalAmount = od.TotalAmount
                                 }).ToList()
                    };
        // var query = from o in _context.Orders
        //             where o.CustomerId == customerid
        //             select new CustomerHistoryViewModel
        //             {
        //                 Date = o.OrderDate,
        //                 OrderType = true,
        //                 PaymentType = o.PaymentMode,
        //                 Totalitems = _context.Dishritems.Where(i => i.Orderid == o.OrderId).Count(),
        //                 TotalAmount = o.TotalAmount
        //             };

        return query.FirstOrDefault();
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

                if(newCustomer.CustomerId != 0)
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
        if(email!=null)
        {
            var customer =await _context.Customers.FirstOrDefaultAsync(c=> c.Email==email);

            if(customer !=null)
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
    catch (System.Exception)
    {
        
        return new CustomerViewModel();
    }
   }

}
