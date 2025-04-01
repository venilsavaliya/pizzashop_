using BLL.Helper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BLL.Services;

public class SectionServices:ISectionServices
{
    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContext;

    private readonly IJwtService _jwtservices;

    private readonly IUserService _userservices;

    public SectionServices(ApplicationDbContext dbcontext, IHttpContextAccessor httpContext, IUserService userService, IJwtService jwtservices)
    {
        _context = dbcontext;
        _httpContext = httpContext;
        _userservices = userService;
        _jwtservices = jwtservices;
    }
    // Get List Of All Sections
    public IEnumerable<SectionNameViewModel> GetSectionList()
    {

        var sections = _context.Sections
                         .Where(c => c.Isdeleted!=true) // Exclude deleted categories
                         .Select(c => new SectionNameViewModel
                         {
                             SectionId = c.SectionId,
                             SectionName = c.SectionName,
                             Description = c.Description
                         })
                         .OrderBy(c => c.SectionId).ToList();

        sections = new List<SectionNameViewModel>(sections);
        return sections;
    }

    // Return list Of Pagination Table List

    public TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {   
        
        TableListPaginationViewModel model = new() { Page = new() };
        searchKeyword = searchKeyword.ToLower();
        // var categoryId = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryid)?.CategoryId;

        var query = from i in _context.Diningtables
                    where i.Isdeleted != true && i.SectionId == sectionid
                    select new TableViewModel
                    {
                        TableId = i.TableId,
                        SectionId = i.SectionId,
                        Name = i.Name,
                        Capacity = i.Capacity,
                        Status = i.Status
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
        model.Sectionid = sectionid;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
        return model;
    }

 

    // Add new Section
    public async Task<AuthResponse> AddSection(AddSectionViewModel model)
    {   
        try{
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        var existingtable = _context.Sections.FirstOrDefault(s => s.SectionName == model.SectionName);

        if(existingtable != null)
        {
            if(existingtable.Isdeleted==true)
            {
                existingtable.Isdeleted = false;
                existingtable.Description = model.Description;
                existingtable.Createdby = userid;
                _context.Sections.Update(existingtable);
                await _context.SaveChangesAsync();
                return new AuthResponse
                {
                    Success = true,
                    Message = "Section Added Succesfully!"
                };
            }
            return new AuthResponse
            {
                Success = false,
                Message = "Section Already Exists!"
            };
        }

        var section = new Section
        {
            SectionName = model.SectionName,
            Description = model.Description,
            Createdby = userid
        };

        _context.Sections.Add(section);
        await _context.SaveChangesAsync();

        return new AuthResponse
            {
                Success = true,
                Message = "Section Added Succesfully!"
            };
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error in AddSection: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Adding Section!"
            };
        }
    }

    // Edit Section
    public async Task<AuthResponse> EditSection(AddSectionViewModel model)
    {   
        try{
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);
        

        var existingsection = _context.Sections.FirstOrDefault(s => s.SectionId == model.Sectionid);

        if(existingsection == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Section Not Found!"
            };
        }
       
            existingsection.SectionName = model.SectionName;
            existingsection.Description = model.Description;
            existingsection.Modifyiedby = userid;
    

        _context.Sections.Update(existingsection);
        await _context.SaveChangesAsync();

        return new AuthResponse
            {
                Success = true,
                Message = "Section Updated Succesfully!"
            };
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error in UpdateSection: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Updating Section!"
            };
        }
    }

    // Delete section 
    public async Task<AuthResponse> DeleteSection(int id)
    {
        var section = _context.Sections.FirstOrDefault(c => c.SectionId == id);

        if (section != null)
        {
            // Fetch all tables that belong to this category
            var items = _context.Diningtables.Where(i => i.SectionId == id).ToList();

            // Mark all items as deleted
            foreach (var item in items)
            {
                item.Isdeleted = true;
            }

            section.Isdeleted = true;

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "section Deleted Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "cant delete section"

            };
        }
    }


    // Add New Table

    public async Task<AuthResponse> AddTable(AddTableViewmodel model)
    {   
        try{
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        var table = new Diningtable
        {
            SectionId = model.SectionId,
            Name= model.Name,
            Capacity=model.Capacity,
            Status=model.Status,
            Createdby = userid
        };

        _context.Diningtables.Add(table);
        await _context.SaveChangesAsync();

        return new AuthResponse
            {
                Success = true,
                Message = "Table Added Succesfully!"
            };
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error in AddTable: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Adding Table!"
            };
        }
    }

    //  Edit Table 
    public async Task<AuthResponse> EditTable(AddTableViewmodel model)
    {   
        try{
        var token = _httpContext.HttpContext.Request.Cookies["jwt"];
        var userid = _userservices.GetUserIdfromToken(token);

        var table = _context.Diningtables.FirstOrDefault(t=> t.TableId == model.TableId);

        
            table.SectionId = model.SectionId;
            table.Name= model.Name;
            table.Capacity=model.Capacity;
            table.Status=model.Status;
            table.Modifyiedby = userid;
        

        _context.Diningtables.Update(table);
        await _context.SaveChangesAsync();

        return new AuthResponse
            {
                Success = true,
                Message = "Table Updated Succesfully!"
            };
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error in UpdateTable: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Updating Table!"
            };
        }
    }

    // Delete single Table
    public async Task<AuthResponse> DeleteTable(int id)
    {
        var item = _context.Diningtables.FirstOrDefault(c => c.TableId == id);

        if (item != null)
        {
            item.Isdeleted = true;

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Table Deleted Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "can't delete table"

            };
        }
    }

    // Delete Multiple Tables

    public async Task<AuthResponse> DeleteTables(List<int> ids)
    {
        try
        {   
            foreach (var i in ids)
            {
                var item = _context.Diningtables.FirstOrDefault(itemInDb => itemInDb.TableId == i);
                item.Isdeleted = true;
                _context.Diningtables.Update(item);
            }
                await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Tables Deleted Succesfully!"
            };
        }
        catch (Exception)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "error in Delete Tables!"
            };
            throw;
        }

    }

 
}
