using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BLL.Services;

public class OrderAppMenuService : IOrderAppMenuService
{
    private readonly ApplicationDbContext _context;
    private readonly IMenuServices _menuservice;



    public OrderAppMenuService(ApplicationDbContext context, IMenuServices menuservice)
    {
        _context = context;
        _menuservice = menuservice;
    }

    // get list of menu item based on category id

    public async Task<List<OrderAppMenuItemViewModel>> GetMenuItem(int catid = 0, string searchkeyword = "", bool isfav = false)
    {
        try
        {
            searchkeyword = searchkeyword?.ToLower() ?? "";

            var query = _context.Items
                .Where(i => i.Isdeleted != true)
                .AsQueryable();



            if (catid != 0)
            {
                query = query.Where(i => i.CategoryId == catid);
            }

            if (!string.IsNullOrWhiteSpace(searchkeyword))
            {
                query = query.Where(i => i.ItemName.ToLower().Contains(searchkeyword));
            }

            if (isfav)
            {
                query = query.Where(i => i.Isfavourite == true);
            }

            var items = await query
                .Select(i => new OrderAppMenuItemViewModel
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Rate = i.Rate,
                    Type = i.Type,
                    Image = i.Image,
                    Isfavourite = i.Isfavourite

                    // Add other fields as necessary
                })
                .OrderBy(i => i.ItemId)
                .ToListAsync();

            return items;
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Get Status Of Order

    public string GetOrderStatus(int orderid)
    {
        try
        {
            if (orderid == 0)
            {
                return "";
            }

            var order = _context.Orders.FirstOrDefault(i => i.OrderId == orderid);

            if (order != null)
            {
                return order.OrderStatus;
            }
            else
            {
                return "";
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }
    public async Task<OrderAppModifierItemList> GetModifierGroupsByItemId(int itemId)
    {
        try
        {
            var result = await _context.Items
     .Where(i => i.ItemId == itemId)
     .Select(i => new OrderAppModifierItemList
     {
         ItemId = i.ItemId,
         ItemName = i.ItemName,
         Modifiergroups = (
             from mgm in _context.ItemModifiergroupMappings
             where mgm.ItemId == i.ItemId && mgm.Isdeleted != true
             join mgMinMax in _context.Itemsmodifiergroupminmaxmappings
                 on new { mgm.ItemId, mgm.ModifierGroupId } equals new { mgMinMax.ItemId, ModifierGroupId = mgMinMax.ModifiergroupId }
             join mg in _context.Modifiersgroups
                 on mgm.ModifierGroupId equals mg.ModifiergroupId
             select new OrderAppModifier
             {
                 ModifiergroupId = mg.ModifiergroupId,
                 Name = mg.Name,
                 MinValue = mgMinMax.MinValue,
                 MaxValue = mgMinMax.MaxValue,
                 ModifierItems = (
                     from mig in _context.Modifieritemsmodifiersgroups
                     where mig.ModifiergroupId == mg.ModifiergroupId
                     join mi in _context.Modifieritems
                         on mig.ModifierId equals mi.ModifierId
                     where mi.Isdeleted != true
                     select new OrderAppModifierItemsDetail
                     {
                         ModifierId = mi.ModifierId,
                         ModifierName = mi.ModifierName,
                         Rate = mi.Rate
                     }).ToList()
             }).ToList()
     }).FirstOrDefaultAsync();

            return result;
        }
        catch (System.Exception)
        {

            throw;
        }

    }

    public async Task<AuthResponse> ChangeStatusOfFavouriteItem(int itemid)
    {
        try
        {
            if (itemid == 0)
            {
                return new AuthResponse
                {
                    Message = "Invalid ItemId!",
                    Success = false
                };
            }

            var item = _context.Items.FirstOrDefault(i => i.ItemId == itemid);

            if (item != null)
            {
                item.Isfavourite = !item.Isfavourite;
            }

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Message = "Favourite status Changed Succefully!",
                Success = true
            };
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<MenuItemModifierGroupMappiingViewModel> GetModifierItemsOfMenuItem(int id)
    {
        try
        {
            if (id == 0)
            {
                return new MenuItemModifierGroupMappiingViewModel();
            }

            var data = new MenuItemModifierGroupMappiingViewModel();
            var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
            data.ItemId = id;
            data.ItemName = item?.ItemName ?? "";
            data.Rate = item?.Rate ?? 0;
            data.TaxPercentage = item?.TaxPercentage ?? 0;


            var modgroupdata = _context.Itemsmodifiergroupminmaxmappings
                                .Include(i => i.Modifiergroup)
                                    .ThenInclude(i => i.Modifieritemsmodifiersgroups)
                                        .ThenInclude(i => i.Modifier);

            data.ModifierGroups = modgroupdata.Where(i => i.ItemId == id).Select(i => new ModifierGroupMinMaxMapping
            {
                ModifiergroupId = i.ModifiergroupId,
                MaxValue = i.MaxValue,
                MinValue = i.MinValue,
                Name = i.Modifiergroup.Name,
                ModifierItems = i.Modifiergroup.Modifieritemsmodifiersgroups
                                .Where(mg => mg.Modifier != null)
                                .Select(mg => new ModifierItemNamePriceViewModel
                                {
                                    ModifierId = mg.Modifier!.ModifierId,
                                    ModifierName = mg.Modifier.ModifierName,
                                    Rate = mg.Modifier.Rate
                                }).ToList()

            }).ToList();

            return data;
        }
        catch (System.Exception)
        {

            throw;
        }
    }


    public async Task<OrderAppMenuMainViewModel> GetOrderDetailByOrderId(int orderid)
    {

        try
        {
            if (orderid == 0)
            {
                return new OrderAppMenuMainViewModel();
            }

            var order = _context.Orders
                        .Include(o => o.Tableorders)
                            .ThenInclude(to => to.Table)
                                .ThenInclude(t => t.Section)
                        .Include(o => o.Dishritems)
                            .ThenInclude(m => m.Dishrmodifiers)
                        .FirstOrDefault(o => o.OrderId == orderid && o.Isdeleted != true);

            OrderAppMenuMainViewModel data = new OrderAppMenuMainViewModel
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderComment = order.Instruction,
                OrderStatus = order.OrderStatus,
                TableList = order.Tableorders.Select(i => new TableCapacityList
                {
                    TableId = i.TableId ?? 0,
                    Capacity = i.Table.Capacity,
                    Name = i.Table.Name,
                }).ToList(),
                OrderItems = order.Dishritems.Select(di => new MenuOrderItemViewModel
                {
                    ItemId = di.Itemid,
                    ItemName = di.Itemname,
                    DishId = di.Dishid,
                    ItemComment = di.Instructions,
                    Rate = di.Itemprice ?? 0,
                    Quantity = di.Quantity ?? 0,
                    TaxPercentage = di.Itemtax ?? 0,
                    ModifierItems = di.Dishrmodifiers.Select(i => new ModifierItemNamePriceViewModel
                    {
                        ModifierId = i.Modifieritemid,
                        ModifierName = i.Modifieritemname ?? "",
                        Rate = i.Modifieritemprice
                    }).ToList()
                }).OrderBy(d => d.DishId).ToList(),
                SectionId = order.Tableorders.Select(i => i.Table?.SectionId).FirstOrDefault() ?? 0,
                SectionName = order.Tableorders.Select(i => i.Table?.Section?.SectionName).FirstOrDefault() ?? "",
                TaxList = _context.Taxes.Where(t => t.Isenable == true && t.Isdeleted != true).Select(i => new TaxViewModel
                {
                    TaxId = i.TaxId,
                    Isdefault = i.Isdefault,
                    Isenable = i.Isenable ?? false,
                    TaxAmount = i.TaxAmount,
                    TaxName = i.TaxName,
                    Type = i.Type
                }).ToList()
            };

            return data;


        }
        catch (System.Exception)
        {

            throw;
        }

    }

    public bool AreModifiersSame(List<ModifierItemNamePriceViewModel> list1, List<ModifierItemNamePriceViewModel> list2)
    {
        try
        {
            var orderedList1 = list1.Where(x => x != null).OrderBy(x => x.ModifierId).ToList();

            var orderedList2 = list2.Where(x => x != null).OrderBy(x => x.ModifierId).ToList();


            var json1 = JsonConvert.SerializeObject(orderedList1);
            var json2 = JsonConvert.SerializeObject(orderedList2);

            return json1 == json2;
        }
        catch (System.Exception ex)
        {

            throw;
        }

    }
    public async Task<AuthResponse> SaveOrderAsync(SaveOrderItemsViewModel model)
    {
        try
        {
             
            // check that order is served or not
            if (model.OrderId != 0)
            {
                var currentorder = _context.Orders.FirstOrDefault(o => o.OrderId == model.OrderId);

                var OrderItems = _context.Dishritems
                    .Where(d => d.Orderid == model.OrderId)
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
                    currentorder.OrderStatus = Constants.OrderServed;
                }
            }
            
            await _context.SaveChangesAsync();

            #region temporary Tax
            // storing tax in order tax table

            var DbTaxList = await _context.Ordertaxes.Where(i=>i.OrderId==model.OrderId).ToListAsync();

            // 1.Remove Tax Record which is not in Model tax list 

            var modelTaxIds = new HashSet<int>(model.TaxList.Select(i => i.TaxId));

            foreach (var tax in DbTaxList)
            {
                if (!modelTaxIds.Contains(tax.Taxid ?? 0))
                {
                    _context.Ordertaxes.Remove(tax);
                }
            }

            // 2. Add or Update Tax Record from incoming model

            var DbTaxIds = new HashSet<int>(_context.Ordertaxes.Select(i => i.Taxid ?? 0));
            foreach (var tax in model.TaxList)
            {
                if (!DbTaxIds.Contains(tax.TaxId))
                {
                    Ordertax ordertax = new Ordertax
                    {
                        OrderId = model.OrderId,
                        Taxid = tax.TaxId,
                        Taxname = tax.TaxName,
                        Taxtype = tax.Type,
                        Taxamount = tax.TaxAmount
                    };

                    await _context.Ordertaxes.AddAsync(ordertax);
                }
                else
                {
                    if (tax.TaxId == 0)
                    {
                        var existingtax = _context.Ordertaxes.FirstOrDefault(i => i.OrderId == model.OrderId && i.Taxid == null);

                        if (existingtax != null)
                        {
                            existingtax.Taxtype = tax.Type;
                            existingtax.Taxname = tax.TaxName;
                            existingtax.Taxamount = tax.TaxAmount;
                        }
                        else
                        {
                            Ordertax ordertax = new Ordertax
                            {
                                OrderId = model.OrderId,
                                Taxid = null,
                                Taxname = tax.TaxName,
                                Taxtype = tax.Type,
                                Taxamount = tax.TaxAmount
                            };

                            await _context.Ordertaxes.AddAsync(ordertax);
                        }

                    }
                    else
                    {
                        var ordertax = await _context.Ordertaxes.FirstOrDefaultAsync(i => i.OrderId == model.OrderId && i.Taxid == tax.TaxId);
                        if (ordertax != null)
                        {
                            ordertax.Taxtype = tax.Type;
                            ordertax.Taxname = tax.TaxName;
                            ordertax.Taxamount = tax.TaxAmount;
                        }
                    }


                }
            }

            await _context.SaveChangesAsync();
            #endregion

            var existingOrders = _context.Dishritems.Where(d => d.Orderid == model.OrderId)
                                    .Include(d => d.Dishrmodifiers);

            var tables = _context.Tableorders.Where(i => i.OrderId == model.OrderId).ToList();

            var runningStatusId = _context.Tablestatuses.FirstOrDefault(i => i.Statusname == Constants.Running)?.Id ?? 0;

            foreach (var t in tables)
            {
                var table = _context.Diningtables.FirstOrDefault(i => i.TableId == t.TableId);

                if (table.Status != runningStatusId)
                {
                    table.Status = runningStatusId;
                    await _context.SaveChangesAsync();
                }
            }


            List<MenuOrderItemViewModel> existingitems = existingOrders.Select(i => new MenuOrderItemViewModel
            {
                DishId = i.Dishid,
                ItemId = i.Itemid,
                ItemComment = i.Instructions,
                ItemName = i.Itemname,
                Quantity = i.Quantity ?? 1,
                ModifierItems = i.Dishrmodifiers.Select(m => new ModifierItemNamePriceViewModel
                {
                    ModifierId = m.Modifieritemid,
                    Rate = m.Modifieritemprice,
                    ModifierName = m.Modifieritemname
                }).ToList(),
            }).ToList();

            // 1. Delete items which are present in DB but not in incoming model
            foreach (var dbItem in existingitems)
            {
                var isPresentinList = model.OrderItems.FirstOrDefault(x => dbItem.ItemId == x.ItemId && AreModifiersSame(x.ModifierItems, dbItem.ModifierItems));

                if (isPresentinList == null)
                {
                    // No matching item found so Delete db item
                    var dishItem = await _context.Dishritems
                                    .Include(d => d.Dishrmodifiers)
                                    .FirstOrDefaultAsync(d => d.Orderid == model.OrderId && d.Dishid == dbItem.DishId);

                    if (dishItem.Readyquantity > 0 || dishItem.Servedquantity > 0)
                    {
                        return new AuthResponse
                        {
                            Message = dishItem.Itemname + " Already Prepared Can't Delete!",
                            Success = false
                        };
                    }

                    if (dishItem != null)
                    {
                        _context.Dishrmodifiers.RemoveRange(dishItem.Dishrmodifiers);
                        _context.Dishritems.Remove(dishItem);
                    }
                }
            }


            // 2. Add or Update items from incoming model
            foreach (var item in model.OrderItems)
            {
                var isPesentIndbItem = existingitems.FirstOrDefault(x => item.ItemId == x.ItemId && AreModifiersSame(x.ModifierItems, item.ModifierItems));

                if (isPesentIndbItem == null)
                {
                    // Not present 
                    Dishritem orderItem = new Dishritem
                    {
                        Itemid = item.ItemId,
                        Orderid = model.OrderId,
                        Quantity = item.Quantity,
                        Pendingquantity = item.Quantity,
                        Itemprice = item.Rate,
                        CategoryId = _context.Items.FirstOrDefault(j => j.ItemId == item.ItemId)?.CategoryId,
                        Itemtax = item.TaxPercentage,
                        Instructions = item.ItemComment,
                        Itemname = item.ItemName
                    };

                    await _context.Dishritems.AddAsync(orderItem);
                    await _context.SaveChangesAsync();

                    foreach (var mod in item.ModifierItems)
                    {
                        Dishrmodifier modifier = new Dishrmodifier
                        {
                            Dishid = orderItem.Dishid,
                            Modifieritemid = mod.ModifierId,
                            Modifieritemprice = (short)mod.Rate,
                            Modifieritemname = mod.ModifierName
                        };

                        await _context.Dishrmodifiers.AddAsync(modifier);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Present 
                    var dbDishItem = await _context.Dishritems.FirstOrDefaultAsync(d => d.Orderid == model.OrderId && d.Itemid == item.ItemId && d.Dishid == isPesentIndbItem.DishId);

                    if (dbDishItem != null)
                    {
                        var readyQty = dbDishItem.Readyquantity;

                        // only update if item quantity is greater than ready quantity of the item
                        if (item.Quantity >= readyQty)
                        {
                            dbDishItem.Quantity = item.Quantity;
                            dbDishItem.Pendingquantity = item.Quantity - (dbDishItem.Readyquantity + dbDishItem.Servedquantity);
                            dbDishItem.Itemprice = item.Rate;
                            dbDishItem.Instructions = item.ItemComment;
                            dbDishItem.Itemtax = item.TaxPercentage;
                            dbDishItem.Itemname = item.ItemName;
                        }
                        else
                        {
                            return new AuthResponse
                            {
                                Message = dbDishItem.Readyquantity + " Items Already Prepared Of " + dbDishItem.Itemname,
                                Success = false
                            };
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }

            // Update Total Amount & Status Of Order

            var order = await _context.Orders.FirstOrDefaultAsync(i => i.OrderId == model.OrderId && model.OrderItems.Any());
            if (order != null)
            {
                order.TotalAmount = model.TotalAmount;
                order.OrderStatus = Constants.OrderInProgress;

                await _context.SaveChangesAsync();
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Order Saved Succesfully!"
            };

        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<AuthResponse> CompleteOrder(SaveOrderItemsViewModel model)
    {
        try
        {
            if (model.OrderId == 0)
            {
                return new AuthResponse
                {
                    Message = "Invalid Order Id!",
                    Success = false
                };
            }
            else
            {
                var order = await _context.Orders.FirstOrDefaultAsync(i => i.OrderId == model.OrderId);

                if (order.OrderStatus != "Served")
                {
                    return new AuthResponse
                    {
                        Message = "Order Is Not Served Yet!",
                        Success = false
                    };
                }

                // Update The Database When Order Is Completed

                #region temporary Tax
                // storing tax in order tax table

                var DbTaxList = await _context.Ordertaxes.ToListAsync();

                // 1.Remove Tax Record which is not in Model tax list 

                var modelTaxIds = new HashSet<int>(model.TaxList.Select(i => i.TaxId));

                foreach (var tax in DbTaxList)
                {
                    if (!modelTaxIds.Contains(tax.Taxid ?? 0))
                    {
                        _context.Ordertaxes.Remove(tax);
                    }
                }

                // 2. Add or Update Tax Record from incoming model

                var DbTaxIds = new HashSet<int>(_context.Ordertaxes.Select(i => i.Taxid ?? 0));
                foreach (var tax in model.TaxList)
                {
                    if (!DbTaxIds.Contains(tax.TaxId))
                    {
                        Ordertax ordertax = new Ordertax
                        {
                            OrderId = model.OrderId,
                            Taxid = tax.TaxId,
                            Taxname = tax.TaxName,
                            Taxtype = tax.Type,
                            Taxamount = tax.TaxAmount
                        };

                        await _context.Ordertaxes.AddAsync(ordertax);
                    }
                    else
                    {
                        if (tax.TaxId == 0)
                        {
                            var existingtax = _context.Ordertaxes.FirstOrDefault(i => i.OrderId == model.OrderId && i.Taxid == null);

                            if (existingtax != null)
                            {
                                existingtax.Taxtype = tax.Type;
                                existingtax.Taxname = tax.TaxName;
                                existingtax.Taxamount = tax.TaxAmount;
                            }
                            else
                            {
                                Ordertax ordertax = new Ordertax
                                {
                                    OrderId = model.OrderId,
                                    Taxid = null,
                                    Taxname = tax.TaxName,
                                    Taxtype = tax.Type,
                                    Taxamount = tax.TaxAmount
                                };

                                await _context.Ordertaxes.AddAsync(ordertax);
                            }

                        }
                        else
                        {
                            var ordertax = await _context.Ordertaxes.FirstOrDefaultAsync(i => i.OrderId == model.OrderId && i.Taxid == tax.TaxId);
                            if (ordertax != null)
                            {
                                ordertax.Taxtype = tax.Type;
                                ordertax.Taxname = tax.TaxName;
                                ordertax.Taxamount = tax.TaxAmount;
                            }
                        }


                    }
                }

                await _context.SaveChangesAsync();
                #endregion
                

                List<Diningtable> tables = _context.Diningtables.Where(i=>i.CurrentOrderId == model.OrderId).ToList();
                foreach (var t in tables)
                {
                    t.CurrentOrderId = null;
                    t.Customerid =null;
                    t.AssignTime = null;
                    t.Status = _context.Tablestatuses.FirstOrDefault(i => i.Statusname == Constants.Available)?.Id ?? 0;
                }


                order.OrderStatus = Constants.OrderCompleted;
                order.CompletedTime = DateTime.Now;
                order.TotalAmount = model.TotalAmount;
                order.PaymentMode = model.PaymentMode;

                //Generating New Invoice For This Order

                Invoice invoice = new Invoice {
                    OrderId = model.OrderId,
                    Paidon = DateTime.Now
                };

                await _context.Invoices.AddAsync(invoice);

                await _context.SaveChangesAsync();

                return new AuthResponse{
                    Message = "Order Completed Successfully!",
                    Success = true
                };
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public int GetReadyQuantityOfItem(int id)
    {
        try
        {
            if (id == 0)
            {
                return 0;
            }

            var orderitem = _context.Dishritems.FirstOrDefault(i => i.Dishid == id);

            return orderitem.Readyquantity + orderitem.Servedquantity ?? 0;
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<OrderCustomerDetailViewModel> GetCustomerDetailsByOrderId(int orderid)
    {
        try
        {
            var customerId = _context.Orders
                .FirstOrDefault(i => i.OrderId == orderid)?.CustomerId;

            var customer = await _context.Customers.FirstOrDefaultAsync(i => i.CustomerId == customerId);

            if (customer == null)
            {
                return new OrderCustomerDetailViewModel();
            }

            OrderCustomerDetailViewModel model = new OrderCustomerDetailViewModel
            {
                OrderId = orderid,
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Mobile = customer.Mobile,
                TotalPerson = customer.Totalperson ?? 1
            };


            return model;
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<AuthResponse> SaveCustomerDetail(OrderCustomerDetailViewModel model)
    {
        try
        {
            if (model.CustomerId == 0)
            {
                return new AuthResponse
                {
                    Message = "Invalid Customer Id!",
                    Success = false
                };
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(i => i.CustomerId == model.CustomerId);

            if (customer != null)
            {
                // Checking For Maximum Capacity Of Customer Table
                var tables = _context.Diningtables.Where(i => i.Customerid == model.CustomerId).ToList();
                int maxCapacity = 0;
                foreach (var t in tables)
                {
                    maxCapacity += t.Capacity;
                }
                if (maxCapacity < model.TotalPerson)
                {
                    return new AuthResponse
                    {
                        Message = "Table(s) Capacity Is Only " + maxCapacity + " Person!",
                        Success = false
                    };
                }
                customer.Totalperson = model.TotalPerson;
                customer.Name = model.Name;
                customer.Mobile = model.Mobile;
            }

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Message = "Customer Details Saved Successfully!",
                Success = true
            };
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<AuthResponse> SaveOrderInstruction(InstructionViewModel model)
    {
        try
        {

            if (model.DishId != 0)
            {
                var dish = await _context.Dishritems.FirstOrDefaultAsync(i => i.Dishid == model.DishId);

                if (dish != null)
                {
                    dish.Instructions = model.Instruction;

                    await _context.SaveChangesAsync();

                    return new AuthResponse
                    {
                        Message = "Instruction Saved Successfully!",
                        Success = true
                    };
                }
                else
                {
                    return new AuthResponse
                    {
                        Message = "Dish Not Found!",
                        Success = false
                    };
                }
            }
            else if (model.OrderId != 0)
            {
                var order = await _context.Orders.FirstOrDefaultAsync(i => i.OrderId == model.OrderId);

                if (order != null)
                {
                    order.Instruction = model.Instruction;

                    await _context.SaveChangesAsync();

                    return new AuthResponse
                    {
                        Message = "Instruction Saved Successfully!",
                        Success = true
                    };
                }
                else
                {
                    return new AuthResponse
                    {
                        Message = "Order Not Found!",
                        Success = false
                    };
                }
            }
            else
            {
                return new AuthResponse
                {
                    Message = "Order Not Found!",
                    Success = false
                };
            }


        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<InstructionViewModel> GetInstruction(int dishid = 0, int orderid = 0, int index = 0, string Instruction = "")
    {
        try
        {
            if (dishid != 0)
            {
                var dish = await _context.Dishritems.FirstOrDefaultAsync(i => i.Dishid == dishid);

                if (dish != null)
                {
                    var model = new InstructionViewModel();
                    model.DishId = dish.Dishid;
                    model.Instruction = dish.Instructions;
                    model.Index = index;

                    return model;
                }
                else
                {
                    return new InstructionViewModel();
                }
            }
            else if (orderid != 0)
            {
                var order = await _context.Orders.FirstOrDefaultAsync(i => i.OrderId == orderid);

                if (order != null)
                {
                    var model = new InstructionViewModel();
                    model.OrderId = order.OrderId;
                    model.Instruction = order.Instruction;
                    model.Index = index;

                    return model;
                }
                else
                {
                    return new InstructionViewModel();
                }
            }
            else
            {
                var model = new InstructionViewModel();
                model.Instruction = Instruction;
                model.Index = index;
                return model;
            }

        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
