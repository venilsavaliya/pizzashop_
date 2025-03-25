namespace BLL.Services;
using BLL.Helper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContext;

    private readonly IJwtService _jwtservices;

    private readonly IUserService _userservices;

    public OrderService(ApplicationDbContext dbcontext, IHttpContextAccessor httpContext, IUserService userService, IJwtService jwtservices)
    {
        _context = dbcontext;
        _httpContext = httpContext;
        _userservices = userService;
        _jwtservices = jwtservices;
    }

    public async Task<OrderListPaginationViewModel> GetOrderList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "", string status = "", DateTime? startDate = null, DateTime? endDate = null)
    {
        searchKeyword = searchKeyword.ToLower();
        OrderListPaginationViewModel model = new() { Page = new() };

        var query = from u in _context.Orders
                    join user in _context.Customers on u.CustomerId equals user.CustomerId
                    select new OrderViewModel
                    {
                        OrderId = u.OrderId,
                        OrderDate = u.OrderDate,
                        CustomerName = user.Name,
                        OrderStatus = u.OrderStatus,
                        PaymentMode = u.PaymentMode,
                        Rating = u.Rating,
                        TotalAmount = u.TotalAmount
                    };

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(u => u.OrderStatus.ToLower().Equals(status.ToLower()));
        }

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u =>
                                 u.OrderId.ToString().Contains(searchKeyword) ||
                                 u.CustomerName.ToLower().Contains(searchKeyword));
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(u => u.OrderDate >= startDate.Value.Date && u.OrderDate.Date <= endDate.Value.Date);
        }

        // Sorting logic
        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
        {
            switch (sortColumn)
            {
                case "Order":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.OrderId) : query.OrderByDescending(u => u.OrderId);
                    break;
                case "Date":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.OrderDate) : query.OrderByDescending(u => u.OrderDate);
                    break;
                case "Customer":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.CustomerName) : query.OrderByDescending(u => u.CustomerName);
                    break;
                case "TotalAmount":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.TotalAmount) : query.OrderByDescending(u => u.TotalAmount);
                    break;
                default:
                    query = query.OrderBy(u => u.OrderId); // **Ensure a default sorting**
                    break;
            }
        }
        else
        {
            query = query.OrderBy(u => u.OrderId); // **Apply a default ordering if no sort is provided**
        }


        // Pagination
        int totalCount = query.Count();
        var usersList = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = usersList;
        model.SortColumn = sortColumn;
        model.SortOrder = sortOrder;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
        return model;
    }




}
