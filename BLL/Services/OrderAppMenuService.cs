using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

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
            data.Rate = item?.Rate??0;
            data.TaxPercentage = item?.TaxPercentage??0;
            

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


}
