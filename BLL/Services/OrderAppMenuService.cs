using BLL.Interfaces;
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

    public async Task<List<OrderAppMenuItemViewModel>> GetMenuItem(int catid = 0, string searchkeyword = "",bool isfav = false)
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

            if(isfav)
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
                .ToListAsync();

            return items;
        }
        catch (Exception)
        {
            throw;
        }
    }


}
