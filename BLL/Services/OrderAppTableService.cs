using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;


public class OrderAppTableService : IOrderAppTableService
{
    private readonly ApplicationDbContext _context;
    private readonly ICustomerService _customerservice;


    public OrderAppTableService(ApplicationDbContext context, ICustomerService customerservice)
    {
        _context = context;
        _customerservice = customerservice;
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

    public async Task<AuthResponse> AssignTableAsync(TableAssignViewModel model)
    {
        try
        {
            var customer = model.Customer;
            
            //this will return id of newly created customer or existing customer
            var customerid = _customerservice.AddCustomer(customer).Result;

            var currenttables = await _context.Diningtables.Where(dt => dt.Isdeleted == false && model.TableId.Contains(dt.TableId)).ToListAsync();
            foreach (var table in currenttables)
            {
                table.Status = _context.Tablestatuses.FirstOrDefault(s => s.Statusname == Constants.Assigned).Id;
                table.Customerid = customerid;
                table.AssignTime = DateTime.Now;
                table.CurrentOrderId = null;

                _context.Diningtables.Update(table);
            }

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Message = "Table Assigned Successfully",
                Success = true
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

