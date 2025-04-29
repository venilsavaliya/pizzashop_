using BLL.Interfaces;
using BLL.Models;
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

    public async Task<List<OrderDetailKOTViewModel>> GetOrderDetailsAsync(int categoryId, bool isPending = false)
    {
        var orders = await _context.Orders
       .Where(o => !o.Isdeleted)
       .Select(order => new OrderDetailKOTViewModel
       {
           OrderId = order.OrderId,
           OrderDate = order.OrderDate,
           Instruction = order.Instruction ?? "",
           SectionName = order.Tableorders
               .Select(to => to.Table!.Section!.SectionName)
               .FirstOrDefault() ?? "",

           TableNames = order.Tableorders
               .Select(to => to.Table!.Name)
               .Distinct()
               .ToList(),

           OrderItems = order.Dishritems
               .Where(di => categoryId == 0 && ((!isPending && di.Readyquantity.HasValue && di.Readyquantity > 0) || (isPending && (di.Pendingquantity.HasValue && di.Pendingquantity > 0))) || di.CategoryId == categoryId &&
                ((!isPending && di.Readyquantity.HasValue && di.Readyquantity > 0) || (isPending && (di.Pendingquantity.HasValue && di.Pendingquantity > 0))))
               .Select(di => new OrderItemKOTViewModel
               {
                   ItemName = di.Itemname ?? "",
                   ItemPrice = di.Itemprice,
                   Instruction = di.Instructions,
                   PendingQuantity = isPending ? di.Pendingquantity : null,
                   ReadyQuantity = !isPending ? di.Readyquantity : null,
                   ModifierList = di.Dishrmodifiers
                       .Select(mod => new OrderModifierViewModel
                       {
                           ModifierItemName = mod.Modifieritemname ?? "",
                           ModifierItemPrice = mod.Modifieritemprice,
                           ModifierItemQuantity = mod.Quantity
                       })
                       .ToList()
               })
               .ToList()
       })
       .ToListAsync();

        return orders;

    }

    public async Task<List<OrderDishKOTViewModel>> GetOrderitemListAsync(int orderid, bool isPending = true)
    {
        var orderitems = await _context.Dishritems
            .Where(o => o.Orderid == orderid && ((!isPending && o.Readyquantity.HasValue && o.Readyquantity > 0) || (isPending && (o.Pendingquantity.HasValue && o.Pendingquantity > 0))))
            .Select(di => new OrderDishKOTViewModel
            {
                ItemName = di.Itemname ?? "",
                DishId = di.Dishid,
                PendingQuantity = isPending ? di.Pendingquantity : null,
                ReadyQuantity = !isPending ? di.Readyquantity : null,
                TotalQuantity =di.Quantity??1,
                ModifierList = di.Dishrmodifiers
                    .Select(mod => new OrderModifierViewModel
                    {
                        ModifierItemName = mod.Modifieritemname ?? "",
                        ModifierItemPrice = mod.Modifieritemprice,
                        ModifierItemQuantity = mod.Quantity
                    })
                    .ToList()
            }).ToListAsync();

        return orderitems;
    }

    public async Task<AuthResponse> UpdateOrderQuantityAsync(List<OrderItemQuantityViewModel> items,bool MarkasPrepared = true)
    {

        foreach (var i in items)
        {
            var orderitem = _context.Dishritems.FirstOrDefault(o => o.Dishid == i.ItemId);

            if (!MarkasPrepared)
            {
                orderitem.Pendingquantity = orderitem.Pendingquantity + i.Quantity;
                orderitem.Readyquantity = orderitem.Readyquantity - i.Quantity;
            }
            else
            {
                orderitem.Readyquantity = orderitem.Readyquantity + i.Quantity;
                orderitem.Pendingquantity = orderitem.Pendingquantity - i.Quantity;
            }

            _context.Dishritems.Update(orderitem);
        }
        await _context.SaveChangesAsync();
        return new AuthResponse
        {
            Success = true,
            Message = "Order quantity updated successfully."
        };
    }
}

