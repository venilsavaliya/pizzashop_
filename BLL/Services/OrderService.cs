namespace BLL.Services;
using BLL.Helper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


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

    // Get Paginated Order List
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

    // Get Order List For Export
    public async Task<IEnumerable<OrderViewModel>> GetOrderListForExport(string searchKeyword = "", string status = "", DateTime? startDate = null, DateTime? endDate = null)
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

        if (!string.IsNullOrEmpty(status) && status != "All Status")
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

        query = query.OrderBy(u => u.OrderId); // **Apply a default ordering if no sort is provided**


        // Get List Of Orders
        var orderList = query.ToList();

        return orderList;
    }

    // Get Data For View Order Detail
    public async Task<OrderDetailViewModel> GetOrderDetailByOrderId(int id)
    {

        var query = from o in _context.Orders where o.OrderId == id
                    join c in _context.Customers on o.CustomerId equals c.CustomerId
                    join i in _context.Invoices on id equals i.OrderId
                    join t in _context.Tableorders on id equals t.OrderId
                    join table in _context.Diningtables on t.TableId equals table.TableId
                    join section in _context.Sections on table.SectionId equals section.SectionId
                    // join oi in _context.Orders on o.OrderId equals oi.OrderId into orderItemsGroup
                    // join ot in _context.Orders on o.OrderId equals ot.OrderId into orderTaxesGroup
                    join dish in _context.Dishritems on id equals dish.Orderid

                    select new OrderDetailViewModel
                    {   
                        OrderId = id,
                        OrderStatus = o.OrderStatus,
                        InvoiceId = i.InvoiceId,
                        Paidon = i.Paidon,
                        Placeon = o.Placeon,
                        CompletedTime = o.CompletedTime,
                        Modifieddate = o.Modifieddate,
                        CustomerName = c.Name,
                        CustomerMobile = c.Mobile,
                        CustomerEmail = c.Email,
                        TotalPerson = o.TotalPerson,
                        TableName = table.Name,
                        SectionName = section.SectionName,
                        ItemList = _context.Dishritems.Where(i=> i.Orderid == o.OrderId).Select(i=> new OrderItemViewModel{
                            ItemName = i.Itemname,
                            ModifierList =_context.Dishrmodifiers.Where(i=> i.Dishid == dish.Dishid).Select(m=> new OrderModifierViewModel{
                                ModifierItemName = m.Modifieritemname,
                                ModifierItemPrice = m.Modifieritemprice,
                                ModifierItemQuantity = m.Quantity
                            }).ToList(),
                            ItemQuantity = i.Quantity,
                            ItemPrice = i.Itemprice

                        }).ToList(),
                        TaxList = _context.Invoicertaxes.Where(j=> j.InvoiceId == i.InvoiceId).Select(t => new OrderTaxViewModel
                        {
                            TaxAmount = t.TaxAmount,
                            TaxName = t.TaxName,
                            TaxType = t.Taxtype
                        }).ToList()
                    };

        return await query.FirstOrDefaultAsync();

    }



}
