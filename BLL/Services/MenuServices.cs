using BLL.Helper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BLL.Services;

public class MenuServices : IMenuServices
{

    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContext;

    private readonly IJwtService _jwtservices;

    private readonly IUserService _userservices;

    public MenuServices(ApplicationDbContext dbcontext, IHttpContextAccessor httpContext, IUserService userService, IJwtService jwtservices)
    {
        _context = dbcontext;
        _httpContext = httpContext;
        _userservices = userService;
        _jwtservices = jwtservices;
    }

    public IEnumerable<CategoryNameViewModel> GetCategoryList()
    {

        var categories = _context.Categories
                         .Where(c => !(bool)c.Isdeleted) // Exclude deleted categories
                         .Select(c => new CategoryNameViewModel
                         {
                             Id = c.CategoryId,
                             Name = c.Name,
                             Description = c.Description
                         })
                         .OrderBy(c => c.Id).ToList();

        categories = new List<CategoryNameViewModel>(categories);
        return categories;
    }

    // For Getting ModifiersGroup List
    public IEnumerable<ModifierGroupNameViewModel> GetModifiersGroupList()
    {

        var ModifierGroups = _context.Modifiersgroups
                         .Where(c => c.Isdeleted != true) // Exclude deleted Modifiergroup
                         .Select(c => new ModifierGroupNameViewModel
                         {
                             ModifiergroupId = c.ModifiergroupId,
                             Name = c.Name,
                             Description = c.Description
                         })
                         .OrderBy(c => c.ModifiergroupId).ToList();

        ModifierGroups = new List<ModifierGroupNameViewModel>(ModifierGroups);
        return ModifierGroups;
    }

    public AuthResponse AddCategory(AddCategoryViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];

        var userid = _userservices.GetUserIdfromToken(token);


        var category = new Category
        {
            Name = model.Name,
            Description = model.Description,
            Createdby = userid,

        };

        _context.Categories.Add(category);
        _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Category Added Succesfully !"
        };
    }

    public AuthResponse EditCategory(AddCategoryViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];

        var userid = _userservices.GetUserIdfromToken(token);

        var ExistingCategory = _context.Categories.FirstOrDefault(c => c.CategoryId.ToString() == model.Id);

        ExistingCategory.Name = model.Name;
        ExistingCategory.Description = model.Description;
        ExistingCategory.Modifyiedby = userid;

        _context.Categories.Update(ExistingCategory);
        _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Category Updated Succesfully"
        };
    }


    public async Task<AuthResponse> DeleteCategory(string id)
    {
        var category = _context.Categories.FirstOrDefault(c => c.CategoryId.ToString() == id);

        if (category != null)
        {
            // Fetch all items that belong to this category
            var items = _context.Items.Where(i => i.CategoryId.ToString() == id).ToList();

            // Mark all items as deleted
            foreach (var item in items)
            {
                item.Isdeleted = true;
            }

            category.Isdeleted = true;

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "category Deleted Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "cant delete category"

            };
        }
    }

    public MenuItemsPaginationViewModel GetItemsListByCategoryId(int categoryid, int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        MenuItemsPaginationViewModel model = new() { Page = new() };
        searchKeyword = searchKeyword.ToLower();
        // var categoryId = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryid)?.CategoryId;

        var query = from i in _context.Items
                    where i.Isdeleted != true && i.CategoryId == categoryid
                    select new ItemViewModel
                    {
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        Type = i.Type,
                        Rate = i.Rate,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        Isavailable = i.Isavailable,
                        Image = i.Image,
                        DefaultTax = i.DefaultTax,
                        TaxPercentage = i.TaxPercentage,
                        ShortCode = i.ShortCode,
                        Description = i.Description
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.ItemName.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        query = query.OrderBy(i => i.ItemName);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.CategoryId = categoryid;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);

        // return new ItemPaginationViewModel
        // {
        //     Category = category,
        //     Items = items,
        //     TotalCount = totalCount,
        //     PageSize = pageSize,
        //     CurrentPage = pageNumber,
        //     TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
        //     StartIndex = (pageNumber - 1) * pageSize + 1,
        //     EndIndex = Math.Min(pageNumber * pageSize, totalCount),
        //     SearchKeyword = searchKeyword
        // };
        return model;
    }

    // Get ModifierItem List from ModifierGroup id

    public ModifierItemsPagination GetModifierItemsListByModifierGroupId(int modifiergroup_id, int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        ModifierItemsPagination model = new() { Page = new() };
        searchKeyword = searchKeyword.ToLower();
        // var categoryId = _context.Modifieritems.FirstOrDefault(c => c.ModifierId == modifiergroup_id)?.ModifierId;

        var query = from i in _context.Modifieritems
                    join m in _context.Modifieritemsmodifiersgroups
                    on i.ModifierId equals m.ModifierId
                    where i.Isdeleted != true && m.ModifiergroupId == modifiergroup_id
                    select new ModifierItemsViewModel
                    {
                        ModifierId = i.ModifierId,
                        Name = i.ModifierName,
                        Rate = i.Rate,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        query = query.OrderBy(i => i.Name);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.ModifierGroupId = modifiergroup_id;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);

        // return new ModifierItemsPagination 
        // {
        //     ModifierGroupId = modifiergroup_id,
        //     Items = items,
        //     TotalCount = totalCount,
        //     PageSize = pageSize,
        //     CurrentPage = pageNumber,
        //     TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
        //     StartIndex = (pageNumber - 1) * pageSize + 1,
        //     EndIndex = Math.Min(pageNumber * pageSize, totalCount),
        //     SearchKeyword = searchKeyword
        // };
        return model;
    }


    // Get all Modifier Items list

    public ModifierItemModalPagination GetAllModifierItemsList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        ModifierItemModalPagination model = new() { Page = new() };
        searchKeyword = searchKeyword.ToLower();
        // var categoryId = _context.Modifieritems.FirstOrDefault(c => c.ModifierId == modifiergroup_id)?.ModifierId;

        var query = from i in _context.Modifieritems
                    where i.Isdeleted != true
                    select new ModifierItemsViewModel
                    {
                        ModifierId = i.ModifierId,
                        Name = i.ModifierName,
                        Rate = i.Rate,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        query = query.OrderBy(i => i.Name);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);

        // return new ModifierPaginationViewModel
        // {
        //     Items = items,
        //     TotalCount = totalCount,
        //     PageSize = pageSize,
        //     CurrentPage = pageNumber,
        //     TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
        //     StartIndex = (pageNumber - 1) * pageSize + 1,
        //     EndIndex = Math.Min(pageNumber * pageSize, totalCount),
        //     SearchKeyword = searchKeyword
        // };

        return model;
    }


    #region Item

    public async Task<AuthResponse> AddNewItem(AddItemViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        string img = "";
        if (model.Image != null)
        {
            img = _userservices.UploadFile(model.Image);

        }

        var item = new Item
        {
            ItemName = model.ItemName,
            Type = model.Type,
            Rate = model.Rate,
            Quantity = model.Quantity,
            CategoryId = model.CategoryId,
            Unit = model.Unit,
            DefaultTax = model.DefaultTax,
            TaxPercentage = model.TaxPercentage,
            ShortCode = model.ShortCode,
            Isavailable = model.Isavailable,
            Description = model.Description,
            Image = img,
            Createdby = userid
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        // Get the new Item ID after saving
        int newItemId = item.ItemId;

        if (model.ModifierGroups != null && model.ModifierGroups.Any())
        {
            var mappings = model.ModifierGroups.Select(mg => new Itemsmodifiergroupminmaxmapping
            {
                ItemId = newItemId,
                ModifiergroupId = mg.ModifierGroupId,
                MinValue = mg.Min,
                MaxValue = mg.Max
            }).ToList();

            _context.Itemsmodifiergroupminmaxmappings.AddRange(mappings);
            await _context.SaveChangesAsync();
        }


        return new AuthResponse
        {
            Success = true,
            Message = "Item Added Successfuuuly"
        };
    }

    public async Task<AuthResponse> EditItem(AddItemViewModel model)
    {
        try
        {

            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var existingitem = _context.Items.FirstOrDefault(i => i.ItemId == model.Id);

            // var modifierGroupIds = model.ModifierGroups
            // .Where(mg => mg.ModifierGroupId != null) // Ensure no null values
            // .Select(mg => mg.ModifierGroupId)
            // .ToList();

            // var existingMappingsList = _context.Itemsmodifiergroupminmaxmappings
            //     .Where(m => m.ItemId == existingitem.ItemId)
            //     .ToList(); // Fetch list before using

            // var toRemove = existingMappingsList
            //     .Where(m => !modifierGroupIds.Contains(m.ModifiergroupId))
            //     .ToList();

            // _context.Itemsmodifiergroupminmaxmappings.RemoveRange(toRemove);

            // await _context.SaveChangesAsync();


            existingitem.ItemName = model.ItemName;
            existingitem.Type = model.Type;
            existingitem.Rate = model.Rate;
            existingitem.Quantity = model.Quantity;
            existingitem.Unit = model.Unit;
            existingitem.DefaultTax = model.DefaultTax;
            existingitem.TaxPercentage = model.TaxPercentage;
            existingitem.ShortCode = model.ShortCode;
            existingitem.Isavailable = model.Isavailable;
            existingitem.Description = model.Description;
            existingitem.Image = model.Image != null ? "sfd" : null;
            existingitem.Modifyiedby = userid;

            _context.Items.Update(existingitem);
            await _context.SaveChangesAsync();

            if (model.ModifierGroups != null && model.ModifierGroups.Any())
            {
                // Remove existing mappings for the item
                var existingMappings = _context.Itemsmodifiergroupminmaxmappings
                    .Where(m => m.ItemId == model.Id)
                    .ToList();

                _context.Itemsmodifiergroupminmaxmappings.RemoveRange(existingMappings);

                // Add new mappings
                var newMappings = model.ModifierGroups.Select(mg => new Itemsmodifiergroupminmaxmapping
                {
                    ItemId = (int)model.Id,  // Ensure model.Id is not null
                    ModifiergroupId = mg.ModifierGroupId,
                    MinValue = mg.Min,
                    MaxValue = mg.Max
                }).ToList();

                _context.Itemsmodifiergroupminmaxmappings.AddRange(newMappings);

                 await _context.SaveChangesAsync();
            }
                



                return new AuthResponse
                {
                    Success = true,
                    Message = "Item Edited Succesfully!"
                };

            }
        catch (Exception)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Error in Item Edit"
            };
        }
    }

    public async Task<AuthResponse> DeleteItems(List<string> ids)
    {
        try
        {
            foreach (var i in ids)
            {
                var item = _context.Items.FirstOrDefault(itemInDb => itemInDb.ItemId.ToString() == i);
                item.Isdeleted = true;
                _context.Items.Update(item);
                await _context.SaveChangesAsync();
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Items Deleted Succesfully!"
            };
        }
        catch (Exception)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "error in Delete item!"
            };
            throw;
        }

    }

    public async Task<AuthResponse> DeleteSingleItem(int id)
    {
        var item = _context.Items.FirstOrDefault(c => c.ItemId == id);

        if (item != null)
        {
            item.Isdeleted = true;

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "item Deleted Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "can't delete category"

            };
        }
    }
    #endregion


    public async Task<AuthResponse> AddNewModifierGroup(AddModifierGroupViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        var ModifierGroup = new Modifiersgroup
        {
            Name = model.Name,
            Description = model.Description,
            Createdby = userid
        };

        _context.Modifiersgroups.Add(ModifierGroup);
        await _context.SaveChangesAsync();

        int newModifierGroupId = ModifierGroup.ModifiergroupId;

        var modifierItemsandgroup = model.ModifieritemsId.Select(itemId => new Modifieritemsmodifiersgroup
        {
            ModifiergroupId = newModifierGroupId,
            ModifierId = itemId
        }).ToList();

        _context.Modifieritemsmodifiersgroups.AddRange(modifierItemsandgroup);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "ModifierGroup Added Successfuuuly"
        };
    }
    public string GetCategoryNameFromId(int id)
    {
        return _context.Categories.FirstOrDefault(c => c.CategoryId == id).Name;
    }

    public List<string> GetModifierNamesByIds(List<string> modifierIds)
    {
        return _context.Modifieritems
                       .Where(m => modifierIds.Contains(m.ModifierId.ToString()))
                       .Select(m => m.ModifierName)
                       .ToList();
    }

    public List<EditModifierGroupItemsViewModel> GetModifierItemListNamesByModifierGroupId(int modifiergroup_id)
    {
        return (from mig in _context.Modifieritemsmodifiersgroups
                join mi in _context.Modifieritems on mig.ModifierId equals mi.ModifierId
                where mig.ModifiergroupId == modifiergroup_id
                select new EditModifierGroupItemsViewModel
                {
                    ModifierId = mi.ModifierId,
                    ModifierName = mi.ModifierName
                }).ToList();
    }

    public ModifierGroupNameViewModel GetModifierGroupNamePVByModifierGroupid(int modifiergroup_id)
    {

        var Modifiergroup = _context.Modifiersgroups.FirstOrDefault(mg => mg.ModifiergroupId==modifiergroup_id);

        var model = new ModifierGroupNameViewModel
        {
            ModifiergroupId = Modifiergroup.ModifiergroupId,
            Name = Modifiergroup.Name
        };
        
        return model;
    }

    public ModifierGroupanditemsViewModel GetModifierItemsByGroupId(int modifiergroup_id)
    {
        var modifierGroup = _context.Modifiersgroups
        .Where(g => g.ModifiergroupId == modifiergroup_id)
        .Select(g => new ModifierGroupanditemsViewModel
        {
            Name = g.Name,
            ModifierGroupId = g.ModifiergroupId,
            items = g.Modifieritemsmodifiersgroups
                .Where(mg => mg.ModifiergroupId == modifiergroup_id)
                .Select(mg => new ModifierItemsAndRate
                {
                    Name = mg.Modifier!.ModifierName, // ModifierName from ModifierItem
                    Rate = mg.Modifier!.Rate ?? 0  // Default to 0 if null
                }).ToList()
        })
        .FirstOrDefault();

        return modifierGroup ?? new ModifierGroupanditemsViewModel(); // Return empty if no data found
    }


    public ModifierGroupanditemsViewModel GetModifierItemswithMinMaxByGroupIdandItemid(int modifiergroup_id, int itemid)
    {
        var modifierGroup = _context.Modifiersgroups
            .Where(g => g.ModifiergroupId == modifiergroup_id)
            .Select(g => new ModifierGroupanditemsViewModel
            {
                Name = g.Name,
                ModifierGroupId = g.ModifiergroupId,

                // Fetch MinValue and MaxValue from the related mapping table
                Minvalue = g.Itemsmodifiergroupminmaxmappings
                    .Where(m => m.ModifiergroupId == modifiergroup_id && m.ItemId == itemid)
                    .Select(m => (int?)m.MinValue) // Cast to nullable to prevent errors if no records found
                    .FirstOrDefault(),

                Maxvalue = g.Itemsmodifiergroupminmaxmappings
                    .Where(m => m.ModifiergroupId == modifiergroup_id && m.ItemId == itemid)
                    .Select(m => (int?)m.MaxValue)
                    .FirstOrDefault(),

                // Fetch Modifier Items
                items = g.Modifieritemsmodifiersgroups
                    .Where(mg => mg.ModifiergroupId == modifiergroup_id)
                    .Select(mg => new ModifierItemsAndRate
                    {
                        Name = mg.Modifier!.ModifierName, // ModifierName from ModifierItem
                        Rate = mg.Modifier!.Rate ?? 0  // Default to 0 if null
                    }).ToList()
            })
            .FirstOrDefault();

        return modifierGroup ?? new ModifierGroupanditemsViewModel(); // Return empty if no data found
    }


    public List<int> GetModifierGroupIdsByItemId(int itemId)
    {
        return _context.Itemsmodifiergroupminmaxmappings
            .Where(m => m.ItemId == itemId) // Filter by ItemId
            .Select(m => m.ModifiergroupId) // Select only ModifiergroupId
            .Distinct() // Ensure unique ModifierGroupIds
            .ToList(); // Convert to List
    }

    // public async Task<List<Itemsmodifiergroupminmaxmapping>> GetItemModifierGroupminMaxMappingByItemId(int itemid)
    // {   

    //     var result = await _context.Itemsmodifiergroupminmaxmappings
    //     .Where(m => m.ItemId == itemid)
    //     .Select(m => new ModifierGroupanditemsViewModel
    //     {
    //         Name = m.Name,
    //         ModifierGroupId = m.ModifiergroupId,
    //         m.MinValue,
    //         m.MaxValue
    //     })
    //     .ToListAsync();

    //     return await _context.Itemsmodifiergroupminmaxmappings
    //        .Where(m => m.ItemId == itemid)
    //        .ToListAsync();
    // }

    public async Task<ModifierGroupanditemsViewModel> GetItemModifierGroupminMaxMappingAsync(int itemId, int modifierGroupId)
    {
        var mapping = await _context.Itemsmodifiergroupminmaxmappings
            .Where(m => m.ItemId == itemId && m.ModifiergroupId == modifierGroupId)
            .Select(m => new ModifierGroupanditemsViewModel
            {
                ModifierGroupId = m.ModifiergroupId,
                Name = m.Modifiergroup.Name,
                Minvalue = m.MinValue,
                Maxvalue = m.MaxValue,
                items = m.Modifiergroup.Modifieritemsmodifiersgroups
                    .Select(i => new ModifierItemsAndRate
                    {
                        Name = i.Modifier!.ModifierName, // ModifierName from ModifierItem
                        Rate = i.Modifier!.Rate ?? 0
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        return mapping;
    }

}
