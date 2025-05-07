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
        // var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderid);
        // if (order != null)
        // {
        //     if (order.OrderStatus == Constants.OrderCancelled)
        //     {
        //         return new List<OrderDetailKOTViewModel>();
        //     }
        // }
        var orders = await _context.Orders
       .Where(o => !o.Isdeleted && o.OrderStatus != Constants.OrderCancelled)
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
                TotalQuantity = di.Quantity ?? 1,
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

    public async Task<AuthResponse> UpdateOrderQuantityAsync(List<OrderItemQuantityViewModel> items, bool MarkasPrepared = true)
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

    public async Task<AuthResponse> UpdateServedQuantityAsync(List<DishItemServeQuantityViewModel> items)
    {
        foreach (var i in items)
        {
            var orderitem = _context.Dishritems.FirstOrDefault(o => o.Dishid == i.DishId);

            if (orderitem != null)
            {
                orderitem.Servedquantity = orderitem.Servedquantity + i.ServeQuantity;
                orderitem.Readyquantity = orderitem.Readyquantity - i.ServeQuantity;
                orderitem.Servingcount = orderitem.Servingcount + 1;

                var orderstarttime = _context.Orders.FirstOrDefault(o => o.OrderId == orderitem.Orderid);
                if (orderstarttime != null)
                {
                    orderitem.Averageservingtime = (int)((orderitem.Averageservingtime + (DateTime.Now - orderstarttime.OrderDate).TotalSeconds) / orderitem.Servingcount);
                }
                else
                {
                    orderitem.Averageservingtime = 0;
                }

            }
        }
        await _context.SaveChangesAsync();

        // check if all item is served or not

        var dishid = items.FirstOrDefault().DishId;

        if (dishid != 0)
        {
            var orderid = _context.Dishritems.FirstOrDefault(d => d.Dishid == dishid).Orderid;

            if (orderid != 0)
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderid);

                var OrderItems = _context.Dishritems
                    .Where(d => d.Orderid == orderid)
                    .ToList();


                bool isOrderServed = true;
                foreach (var item in OrderItems)
                {
                    var dishitem = _context.Dishritems.FirstOrDefault(d => d.Dishid == item.Dishid);
                    if (dishitem != null && dishitem.Pendingquantity > 0)
                    {
                        isOrderServed = false;
                        break;
                    }
                }

                if (isOrderServed)
                {
                    order.OrderStatus = Constants.OrderServed;
                }
            }

            await _context.SaveChangesAsync();
        }
        return new AuthResponse
        {
            Success = true,
            Message = "Item(s) Served successfully."
        };
    }
}

