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

    public async Task<AuthResponse> AddCategory(AddCategoryViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];

        var userid = _userservices.GetUserIdfromToken(token);


        var category = new Category
        {
            Name = model.Name,
            Description = model.Description,
            Createdby = userid,

        };

        var existingcategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower());

        if (existingcategory != null)
        {

            if (existingcategory.Isdeleted != true)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Category Already Existed!"
                };
            }
            existingcategory.Isdeleted = false;
            existingcategory.Description = model.Description;

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Category Added Succesfully !"
            };
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Category Added Succesfully !"
        };
    }

    public async Task<AuthResponse> EditCategory(AddCategoryViewModel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];

            var userid = _userservices.GetUserIdfromToken(token);

            var ExistingCategory = _context.Categories.FirstOrDefault(c => c.CategoryId.ToString() == model.Id);

            //finding existing name in the category list
            var existingcategoryname = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower() && c.CategoryId.ToString() != model.Id);

            if (existingcategoryname != null)
            {

                if (existingcategoryname.Isdeleted != true)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Category Already Existed!"
                    };
                }
                ExistingCategory.Name = model.Name;
                ExistingCategory.Description = model.Description;
                ExistingCategory.Modifyiedby = userid;

                _context.Categories.Update(ExistingCategory);
                await _context.SaveChangesAsync();

                return new AuthResponse
                {
                    Success = true,
                    Message = "Category Updated Succesfully !"
                };
            }

            ExistingCategory.Name = model.Name;
            ExistingCategory.Description = model.Description;
            ExistingCategory.Modifyiedby = userid;

            _context.Categories.Update(ExistingCategory);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Category Updated Succesfully"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error In Edit Category :" + ex.Message);
            return new AuthResponse
            {
                Success = false,
                Message = "Error In Edit Category"
            };
        }
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

       // if pagenumber is exceed the limit page than .
        var maxPageNumber = (int)Math.Ceiling((double)totalCount / pageSize);
        if (pageNumber > maxPageNumber && totalCount!=0)
        {
            pageNumber = maxPageNumber;
        }

        query = query.OrderBy(i => i.ItemName);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.CategoryId = categoryid;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);

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
                        Description = i.Description
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();

        // if pagenumber is exceed the limit page than.
        
        var maxPageNumber = (int)Math.Ceiling((double)totalCount / pageSize);
        if (pageNumber > maxPageNumber && totalCount!=0)
        {
            pageNumber = maxPageNumber;
        }
        query = query.OrderBy(i => i.Name);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.ModifierGroupId = modifiergroup_id;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
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

        var existingnameitem = await _context.Items.FirstOrDefaultAsync(i => i.ItemName.ToLower() == model.ItemName.ToLower() && i.CategoryId == model.CategoryId);

        if (existingnameitem != null)
        {
            if (existingnameitem.Isdeleted == true)
            {
                existingnameitem.Isdeleted = false;
                existingnameitem.ItemName = model.ItemName;
                existingnameitem.Type = model.Type;
                existingnameitem.Rate = model.Rate;
                existingnameitem.Quantity = model.Quantity;
                existingnameitem.CategoryId = model.CategoryId;
                existingnameitem.Unit = model.Unit;
                existingnameitem.DefaultTax = model.DefaultTax;
                existingnameitem.TaxPercentage = model.TaxPercentage;
                existingnameitem.ShortCode = model.ShortCode;
                existingnameitem.Isavailable = model.Isavailable;
                existingnameitem.Description = model.Description;
                existingnameitem.Image = img;
                existingnameitem.Createdby = userid;

                _context.Items.Update(existingnameitem);
                await _context.SaveChangesAsync();

                return new AuthResponse
                {
                    Success = true,
                    Message = "Item Added Succefully!"
                };
            }
            else
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Item With Same Name Existed!"
                };
            }

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
            Message = "Item Added Successfully"
        };
    }

    public async Task<AuthResponse> EditItem(AddItemViewModel model)
    {
        try
        {

            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var existingitem = _context.Items.FirstOrDefault(i => i.ItemId == model.Id);

            var existincategorywithsamename = _context.Items.FirstOrDefault(i => i.ItemId != model.Id && i.ItemName.ToLower() == model.ItemName.ToLower() && i.Isdeleted != true);

            string img = "";
            if (model.Image != null)
            {
                img = _userservices.UploadFile(model.Image);

            }

            if (existingitem != null && existincategorywithsamename == null)
            {
                existingitem.CategoryId = model.CategoryId;
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
                if (model.Image != null)
                {
                    existingitem.Image = img;
                }
                existingitem.Modifyiedby = userid;

                _context.Items.Update(existingitem);
                await _context.SaveChangesAsync();

            }
            else if (existincategorywithsamename != null && existingitem != null)
            {
                if (existincategorywithsamename.Isdeleted == true)
                {
                    existincategorywithsamename.CategoryId = model.CategoryId;
                    existincategorywithsamename.ItemName = model.ItemName;
                    existincategorywithsamename.Type = model.Type;
                    existincategorywithsamename.Rate = model.Rate;
                    existincategorywithsamename.Quantity = model.Quantity;
                    existincategorywithsamename.Unit = model.Unit;
                    existincategorywithsamename.DefaultTax = model.DefaultTax;
                    existincategorywithsamename.TaxPercentage = model.TaxPercentage;
                    existincategorywithsamename.ShortCode = model.ShortCode;
                    existincategorywithsamename.Isavailable = model.Isavailable;
                    existincategorywithsamename.Description = model.Description;
                    existincategorywithsamename.Image = model.Image != null ? "sfd" : null;
                    existincategorywithsamename.Modifyiedby = userid;
                }
                else
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Item with same name Existed!"
                    };
                }
            }
            else
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "No Item Found!"
                };
            }

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


    // public async Task<AuthResponse> AddNewModifierGroup(AddModifierGroupViewModel model)
    // {
    //     var token = _httpContext.HttpContext.Request.Cookies["jwt"];
    //     var userid = _userservices.GetUserIdfromToken(token);

    //     var ModifierGroup = new Modifiersgroup
    //     {
    //         Name = model.Name,
    //         Description = model.Description,
    //         Createdby = userid
    //     };

    //     _context.Modifiersgroups.Add(ModifierGroup);
    //     await _context.SaveChangesAsync();

    //     int newModifierGroupId = ModifierGroup.ModifiergroupId;

    //     var modifierItemsandgroup = model.ModifieritemsId.Select(itemId => new Modifieritemsmodifiersgroup
    //     {
    //         ModifiergroupId = newModifierGroupId,
    //         ModifierId = itemId
    //     }).ToList();

    //     _context.Modifieritemsmodifiersgroups.AddRange(modifierItemsandgroup);
    //     await _context.SaveChangesAsync();

    //     return new AuthResponse
    //     {
    //         Success = true,
    //         Message = "ModifierGroup Added Successfuuuly"
    //     };
    // }

    // service for edit modifier group 
    public async Task<AuthResponse> EditModifierGroup(AddModifierGroupViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        // Check if a modifier group with the same name already exists (excluding the current one)
        bool isDuplicate = await _context.Modifiersgroups
            .AnyAsync(mg => mg.Name.ToLower() == model.Name.ToLower() && mg.ModifiergroupId != model.ModifierId && mg.Isdeleted == false);

        if (isDuplicate)
        {
            return new AuthResponse { Success = false, Message = "A modifier group with the same name already exists." };
        }

        var modifierGroup = await _context.Modifiersgroups
            .Include(mg => mg.Modifieritemsmodifiersgroups)
            .FirstOrDefaultAsync(mg => mg.ModifiergroupId == model.ModifierId);

        if (modifierGroup == null)
        {
            return new AuthResponse { Success = false, Message = "Modifier group not found." };
        }

        // Update basic details
        modifierGroup.Name = model.Name;
        modifierGroup.Description = model.Description;
        // modifierGroup.Modifieddate = DateTime.UtcNow;
        modifierGroup.Modifyiedby = userid;

        // Update Modifier Items Mapping
        var existingMappings = modifierGroup.Modifieritemsmodifiersgroups.ToList();

        // Remove old modifier items that are not in the new list
        foreach (var mapping in existingMappings)
        {
            if (!model.ModifierItems.Contains(mapping.ModifierId ?? 0))
            {
                _context.Modifieritemsmodifiersgroups.Remove(mapping);
            }
        }

        // Add new modifier items
        foreach (var modifierId in model.ModifierItems)
        {
            if (!existingMappings.Any(m => m.ModifierId == modifierId))
            {
                _context.Modifieritemsmodifiersgroups.Add(new Modifieritemsmodifiersgroup
                {
                    ModifiergroupId = model.ModifierId,
                    ModifierId = modifierId
                });
            }
        }

        // Save changes to database
        await _context.SaveChangesAsync();

        return new AuthResponse { Success = true, Message = "Modifier group updated successfully." };

    }

    public async Task<AuthResponse> AddModifierGroup(AddModifierGroupViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userId = _userservices.GetUserIdfromToken(token);

        // Check if a modifier group with the same name already exists (case insensitive)
        var existingModifierGroup = await _context.Modifiersgroups
            .FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower());

        if (existingModifierGroup != null && existingModifierGroup.Isdeleted == false)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Modifier Group already exists!"
            };
        }

        // Create a new modifier group
        var newModifierGroup = new Modifiersgroup
        {
            Name = model.Name,
            Description = model.Description,
            Createdby = userId,  // Assuming you have a CreatedBy field
        };

        var existingmodifiergroup = await _context.Modifiersgroups.FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower());

        if (existingmodifiergroup != null)
        {
            if (existingmodifiergroup.Isdeleted != true)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Modifier Group Already Existed!"
                };
            }
            existingmodifiergroup.Isdeleted = false;
            existingmodifiergroup.Description = model.Description;

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Modifier Group Added Succesfully !"
            };
        }
        // Add the new modifier group to the context
        _context.Modifiersgroups.Add(newModifierGroup);
        await _context.SaveChangesAsync(); // Save first to get the generated ID

        // Add associated modifier items
        if (model.ModifierItems != null && model.ModifierItems.Any())
        {
            var modifierItemMappings = model.ModifierItems.Select(modifierId => new Modifieritemsmodifiersgroup
            {
                ModifiergroupId = newModifierGroup.ModifiergroupId,
                ModifierId = modifierId
            }).ToList();

            _context.Modifieritemsmodifiersgroups.AddRange(modifierItemMappings);
            await _context.SaveChangesAsync();
        }

        return new AuthResponse { Success = true, Message = "Modifier group added successfully." };
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

    public List<ModifierGroupNameViewModel> GetModifierItemListNamesByModifierGroupId(int modifiergroup_id)
    {
        return (from mig in _context.Modifieritemsmodifiersgroups
                join mi in _context.Modifieritems on mig.ModifierId equals mi.ModifierId
                where mig.ModifiergroupId == modifiergroup_id
                select new ModifierGroupNameViewModel
                {
                    ModifiergroupId = mi.ModifierId,
                    Name = mi.ModifierName
                }).ToList();
    }

    public ModifierItemNameViewModel GetModifierItemNamesByModifierItemId(int modifieritem_id)
    {
        //  return (from mig in _context.Modifieritemsmodifiersgroups
        //     join mi in _context.Modifieritems on mig.ModifierId equals mi.ModifierId
        //     where mig.ModifiergroupId == modifieritem_id
        //     select new ModifierGroupNameViewModel
        //     {
        //         Itemid = mi.ModifierId,
        //         Name = mi.ModifierName 
        //     }).FirstOrDefault();

        return _context.Modifieritems
        .Where(i => i.ModifierId == modifieritem_id)
        .Select(i => new ModifierItemNameViewModel
        {
            Itemid = i.ModifierId,  // Assuming ModifierId corresponds to Itemid
            Name = i.ModifierName
        })
        .FirstOrDefault();
    }

    public ModifierGroupNameViewModel GetModifierGroupNamePVByModifierGroupid(int modifiergroup_id)
    {

        var Modifiergroup = _context.Modifiersgroups.FirstOrDefault(mg => mg.ModifiergroupId == modifiergroup_id);

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

    public async Task<AuthResponse> DeleteModifierGroupById(string id)
    {
        if (!int.TryParse(id, out int modifierGroupId))
        {
            return new AuthResponse { Success = false, Message = "Invalid ID format." };
        }

        var modifierGroup = await _context.Modifiersgroups.FindAsync(modifierGroupId);
        if (modifierGroup == null)
        {
            return new AuthResponse { Success = false, Message = "Modifier Group not found." };
        }

        try
        {
            // Remove related records from Modifieritemsmodifiersgroup
            var relatedModifierItems = _context.Modifieritemsmodifiersgroups
                .Where(m => m.ModifiergroupId == modifierGroupId);
            _context.Modifieritemsmodifiersgroups.RemoveRange(relatedModifierItems);

            // Remove related records from ItemModifiergroupMappings
            var relatedItemMappings = _context.ItemModifiergroupMappings
                .Where(m => m.ModifierGroupId == modifierGroupId);
            _context.ItemModifiergroupMappings.RemoveRange(relatedItemMappings);

            // Save changes to remove dependencies
            await _context.SaveChangesAsync();

            // Now delete the modifier group
            _context.Modifiersgroups.Remove(modifierGroup);
            await _context.SaveChangesAsync();

            return new AuthResponse { Success = true, Message = "Modifier Group deleted successfully." };
        }
        catch (Exception ex)
        {
            return new AuthResponse { Success = false, Message = "Error deleting Modifier Group." };
        }
    }

    public async Task<AuthResponse> AddModifierItem(AddModifierItemViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);


        var item = new Modifieritem
        {
            ModifierName = model.ModifierName,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Unit = model.Unit,
            Description = model.Description,
            Createdby = userid
        };

        _context.Modifieritems.Add(item);
        await _context.SaveChangesAsync();

        // Get the new Item ID after saving
        int newItemId = item.ModifierId;

        foreach (var groupId in model.ModifierGroupid)
        {
            var mapping = new Modifieritemsmodifiersgroup
            {
                ModifiergroupId = groupId,
                ModifierId = newItemId
            };
            _context.Modifieritemsmodifiersgroups.Add(mapping);
        }
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "ModifierItem Added Successfuuuly"
        };
    }
    public async Task<AuthResponse> EditModifierItem(AddModifierItemViewModel model)
    {
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        var item = _context.Modifieritems.FirstOrDefault(mi => mi.ModifierId == model.ModifierId);

        item.ModifierName = model.ModifierName;
        item.Rate = model.Rate;
        item.Quantity = model.Quantity;
        item.Unit = model.Unit;
        item.Description = model.Description;
        item.Modifyiedby = userid;


        var mapping = _context.Modifieritemsmodifiersgroups.Where(m => m.ModifierId == model.ModifierId).ToList();
        if (mapping != null)
        {
            _context.Modifieritemsmodifiersgroups.RemoveRange(mapping);
        }
        foreach (var groupId in model.ModifierGroupid)
        {
            var newmapping = new Modifieritemsmodifiersgroup
            {
                ModifiergroupId = groupId,
                ModifierId = model.ModifierId
            };
            _context.Modifieritemsmodifiersgroups.Add(newmapping);
        }
        _context.Modifieritems.Update(item);

        await _context.SaveChangesAsync();


        return new AuthResponse
        {
            Success = true,
            Message = "ModifierItem Added Successfuuuly"
        };
    }

    public async Task<AuthResponse> DeleteModifierItemById(int modifierid, int modifiergroupid)
    {
        try
        {
            var mapping = _context.Modifieritemsmodifiersgroups.FirstOrDefault(i => i.ModifiergroupId == modifiergroupid && i.ModifierId == modifierid);

            if (mapping != null)
            {
                _context.Modifieritemsmodifiersgroups.Remove(mapping);
                await _context.SaveChangesAsync();
            }
            return new AuthResponse { Success = true, Message = "Modifier Item deleted successfully." };
        }
        catch (Exception)
        {
            return new AuthResponse { Success = false, Message = "Error in Deleting Modifier Item!" };
        }


    }

    public async Task<List<object>> GetModifierGroupIdListByModifierItemId(int modifierItemId)
    {
        return await _context.Modifieritemsmodifiersgroups
            .Where(m => m.ModifierId == modifierItemId)
            .Select(m => new
            {
                id = m.ModifiergroupId ?? 0,
                name = m.Modifiergroup.Name
            })
            .ToListAsync<object>();
    }

    public async Task<AuthResponse> DeleteModifierItems(int ModifierGroupid, List<string> ids)
    {
        try
        {
            foreach (var i in ids)
            {
                // var item = _context.Items.FirstOrDefault(itemInDb => itemInDb.ItemId.ToString() == i);
                var item = _context.Modifieritemsmodifiersgroups.FirstOrDefault(itemInDb => itemInDb.ModifierId.ToString() == i && itemInDb.ModifiergroupId == ModifierGroupid);

                if (item != null)
                {

                    _context.Modifieritemsmodifiersgroups.Remove(item);
                }
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


}
