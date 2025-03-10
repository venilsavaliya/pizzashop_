using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
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
                             Id = c.CategoryId.ToString(),
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
                         .Where(c => !(bool)c.Isdeleted) // Exclude deleted Modifiergroup
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


    public AuthResponse DeleteCategory(string id)
    {
        var category = _context.Categories.FirstOrDefault(c => c.CategoryId.ToString() == id);

        if (category != null)
        {
            category.Isdeleted = true;
            _context.SaveChangesAsync();

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

    public ItemPaginationViewModel GetItemsListByCategoryName(string category, int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        searchKeyword = searchKeyword.ToLower();
        var categoryId = _context.Categories.FirstOrDefault(c => c.Name == category)?.CategoryId;

        var query = from i in _context.Items
                    where i.Isdeleted != true && i.CategoryId == categoryId
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

        return new ItemPaginationViewModel
        {   Category=category,
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            StartIndex = (pageNumber - 1) * pageSize + 1,
            EndIndex = Math.Min(pageNumber * pageSize, totalCount),
            SearchKeyword = searchKeyword
        };
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

            existingitem.ItemId = (int)model.Id;
            existingitem.ItemName = model.ItemName;
            existingitem.Type = model.Type;
            existingitem.Rate = model.Rate;
            existingitem.Quantity = model.Quantity;
            existingitem.CategoryId = model.CategoryId;
            existingitem.Unit = model.Unit;
            existingitem.DefaultTax = model.DefaultTax;
            existingitem.TaxPercentage = model.TaxPercentage;
            existingitem.ShortCode = model.ShortCode;
            existingitem.Isavailable = model.Isavailable;
            existingitem.Description = model.Description;
            existingitem.Image = "";
            existingitem.Modifyiedby = userid;

            _context.Items.Update(existingitem);
            await _context.SaveChangesAsync();

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

    #endregion

    public string GetCategoryNameFromId(int id)
    {
        return _context.Categories.FirstOrDefault(c => c.CategoryId == id).Name;
    }
}
