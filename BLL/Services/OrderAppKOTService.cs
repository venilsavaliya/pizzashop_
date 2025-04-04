using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class OrderAppKOTService : IOrderAppKOTService
{
    private readonly ApplicationDbContext _context;
    
    public OrderAppKOTService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryNameViewModel>> GetCategoriesAsync()
    {
        var categories = await _context.Categories
            .Where(c => c.Isdeleted == false) 
            .Select(c => new CategoryNameViewModel
            {
                Id = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();

        return categories;
    }

      public async Task<OrderDetailKOTViewModel> GetOrderDetailsAsync(int categoryId, bool quantityType = false)
    {
        var orderDetails = await _context.Dishritems
            .Include(d => d.Dishrmodifiers)
            .Include(d => d.Order)
            .ThenInclude(o => o.Tableorders)
            .ThenInclude(t => t.Table)
            .Join(_context.Sections,
                dish => dish.Order.Tableorders.FirstOrDefault().Table.SectionId,
                section => section.SectionId,
                (dish, section) => new { dish, section })
            .Where(x => x.dish.Item.CategoryId == categoryId) // Filter by category ID
            .GroupBy(x => new { x.dish.Orderid, x.dish.Order.OrderDate, x.section.SectionName, x.dish.Order.Instruction })
            .Select(g => new OrderDetailKOTViewModel
            {
                OrderId = g.Key.Orderid,
                OrderDate = g.Key.OrderDate,
                SectionName = g.Key.SectionName,
                Instruction = g.Key.Instruction ?? string.Empty,
                TableNames = g.SelectMany(x => x.dish.Order.Tableorders)
                              .Select(to => to.Table.Name)
                              .Distinct()
                              .ToList(),
                OrderItems = g.Select(x => new OrderItemKOTViewModel
                {
                    ItemName = x.dish.Itemname ?? "Unknown Item",
                    PendingQuantity = x.dish.Pendingquantity,
                    ReadyQuantity = x.dish.Readyquantity,
                    ItemPrice = x.dish.Itemprice,
                    Instruction = x.dish.Order.Instruction,
                    ModifierList = x.dish.Dishrmodifiers.Select(m => new OrderModifierViewModel
                    {
                        ModifierItemName = m.Modifieritemname ?? "Unknown Modifier",
                        ModifierItemPrice = m.Modifieritemprice,
                        ModifierItemQuantity = m.Quantity
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (orderDetails == null)
        {
            throw new Exception("Order details not found for the given category.");
        }

        return orderDetails;
    }
}

