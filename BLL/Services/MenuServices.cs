using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BLL.Services;

public class MenuServices : IMenuServices
{

    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContext;

    private readonly IUserService _userservices;

    public MenuServices(ApplicationDbContext dbcontext, IHttpContextAccessor httpContext, IUserService userService)
    {
        _context = dbcontext;
        _httpContext = httpContext;
        _userservices = userService;
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

    public ItemPaginationViewModel GetItemsListByCategoryName(string category,int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {   searchKeyword = searchKeyword.ToLower();
        var categoryId = _context.Categories.FirstOrDefault(c => c.Name == category)?.CategoryId;
        // var items = _context.Items.Where(i => i.Isdeleted != true && catagoryId == i.CategoryId).Select(i => new ItemViewModel
        // {
        //     ItemId = i.ItemId,
        //     ItemName = i.ItemName,
        //     Type = i.Type,
        //     Rate = i.Rate,
        //     Quantity = i.Quantity,
        //     Unit = i.Unit,
        //     Isavailable = i.Isavailable,
        //     Image = i.Image
        // }).ToList();



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
                        Image = i.Image
                    };
        
        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i=>i.ItemName.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

         return new ItemPaginationViewModel
        {
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

}
