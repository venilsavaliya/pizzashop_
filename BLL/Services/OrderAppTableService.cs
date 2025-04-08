using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;


public class OrderAppTableService : IOrderAppTableService
{
    private readonly ApplicationDbContext _context;


    public OrderAppTableService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<OrderAppTableViewModel>> GetOrderAppTableAndSectionList()
    {
        var orderAppTableList = await _context.Sections
            .Where(s=>s.Isdeleted == false)
            .Select(i=> new OrderAppTableViewModel{
                SectionId = i.SectionId,
                SectionName = i.SectionName,
                AvailableCount = _context.Diningtables.Where(t=> t.SectionId == i.SectionId && t.Isdeleted == false && t.Status== _context.Tablestatuses.FirstOrDefault(s=> s.Statusname == Constants.Available).Id).Count(),
                RunningCount = _context.Diningtables.Where(t=> t.SectionId == i.SectionId && t.Isdeleted == false && t.Status== _context.Tablestatuses.FirstOrDefault(s=> s.Statusname == Constants.Running).Id).Count(),
                AssignedCount = _context.Diningtables.Where(t=> t.SectionId == i.SectionId && t.Isdeleted == false && t.Status== _context.Tablestatuses.FirstOrDefault(s=> s.Statusname == Constants.Assigned).Id).Count(),
                Tables = _context.Diningtables
                    .Where(dt=> dt.SectionId == i.SectionId && dt.Isdeleted == false)
                    .Select(dt => new OrderAppTable
                    {
                        TableId = dt.TableId,
                        Name = dt.Name,
                        Amount = _context.Orders.Where(o=> o.OrderId == dt.CurrentOrderId && o.Isdeleted == false).Select(o=> o.TotalAmount).FirstOrDefault(),
                        Capacity = dt.Capacity,
                        Status = dt.Status,
                        AssignTime = dt.AssignTime
                    }).ToList()
            }).ToListAsync();

        return orderAppTableList;
    }

}

