using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


public class OrderAppTableService : IOrderAppTableService
{
    private readonly ApplicationDbContext _context;
    private readonly ICustomerService _customerservice;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IUserService _userservices;


    public OrderAppTableService(ApplicationDbContext context, ICustomerService customerservice, IHttpContextAccessor httpContext, IUserService userService)
    {
        _context = context;
        _customerservice = customerservice;
        _httpContext = httpContext;
        _userservices = userService;
    }

    public async Task<List<OrderAppTableViewModel>> GetOrderAppTableAndSectionList()
    {
        var orderAppTableList = await _context.Sections
            .Where(s => s.Isdeleted == false)
            .Select(i => new OrderAppTableViewModel
            {
                SectionId = i.SectionId,
                SectionName = i.SectionName,
                AvailableCount = _context.Diningtables.Where(t => t.SectionId == i.SectionId && t.Isdeleted == false && t.Status == _context.Tablestatuses.FirstOrDefault(s => s.Statusname == Constants.Available).Id).Count(),
                RunningCount = _context.Diningtables.Where(t => t.SectionId == i.SectionId && t.Isdeleted == false && t.Status == _context.Tablestatuses.FirstOrDefault(s => s.Statusname == Constants.Running).Id).Count(),
                AssignedCount = _context.Diningtables.Where(t => t.SectionId == i.SectionId && t.Isdeleted == false && t.Status == _context.Tablestatuses.FirstOrDefault(s => s.Statusname == Constants.Assigned).Id).Count(),
                Tables = _context.Diningtables
                    .Where(dt => dt.SectionId == i.SectionId && dt.Isdeleted == false)
                    .Select(dt => new OrderAppTable
                    {
                        TableId = dt.TableId,
                        Name = dt.Name,
                        Amount = _context.Orders.Where(o => o.OrderId == dt.CurrentOrderId && o.Isdeleted == false).Select(o => o.TotalAmount).FirstOrDefault(),
                        Capacity = dt.Capacity,
                        Status = dt.Status,
                        AssignTime = dt.AssignTime
                    }).ToList()
            }).ToListAsync();

        return orderAppTableList;
    }

    public async Task<int> AssignTableAsync(TableAssignViewModel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var customerid = 0;
            // if token id present then it mean existing waiting token would be assigned a table
            if (model.Tokenid != null)
            {
                var waitingtokendetail = await _context.Waitingtokens.FirstOrDefaultAsync(i => i.Tokenid == model.Tokenid);
                waitingtokendetail.Completiontime = DateTime.Now;
                customerid = waitingtokendetail.Customerid;
            }
            // table assign to the new customer 
            else
            {
                var customer = model.Customer;

                if (customer != null)
                {   
                    var customerInDB = await _context.Customers.FirstOrDefaultAsync(c=>c.Email == customer.Email);
                    customerid = customerInDB?.CustomerId ?? 0;
                    
                    //Check If Customer Is Already in Waiting List For Another Section
                    var isWaiting = await _context.Waitingtokens.FirstOrDefaultAsync(i => i.Customerid == customerid && i.Isdeleted != true && i.Completiontime == null);

                    var isAlreadyAssigned = await _context.Diningtables.FirstOrDefaultAsync(i=> i.Customerid == customerid && i.Isdeleted !=true);

                    if (isWaiting != null || isAlreadyAssigned !=null)
                    {
                        return 0;
                    }
                }
                //this will return id of newly created customer or existing customer
                customerid = await _customerservice.AddCustomer(customer);


            }

            // this will create new order for the customer 
            var order = new Order();
            order.CustomerId = customerid;
            order.OrderStatus = "Pending";
            order.TotalPerson = model.Customer.TotalPerson;
            order.PaymentMode = "Pending";
            order.Createdby = userid;

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            var currenttables = await _context.Diningtables.Where(dt => dt.Isdeleted == false && model.TableId.Contains(dt.TableId)).ToListAsync();
            foreach (var table in currenttables)
            {
                table.Status = _context.Tablestatuses.FirstOrDefault(s => s.Statusname == Constants.Assigned).Id;
                table.Customerid = customerid;
                table.AssignTime = DateTime.Now;
                table.CurrentOrderId = order.OrderId;

                //this will create table order mapping entry
                var tableOrderMapping = new Tableorder();
                tableOrderMapping.TableId = table.TableId;
                tableOrderMapping.OrderId = order.OrderId;

                _context.Tableorders.Add(tableOrderMapping);

                _context.Diningtables.Update(table);
            }

            await _context.SaveChangesAsync();

            return order.OrderId;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public int GetOrderIdOfTable(int tableid)
    {
        return _context.Diningtables.First(i => i.TableId == tableid).CurrentOrderId ?? 0;
    }

}

