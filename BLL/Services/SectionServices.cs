using BLL.Helper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class SectionServices : ISectionServices
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

    // Get List Of All Table status
    public IEnumerable<Tablestatus> GetTableStatusList()
    {
        var tableStatus = _context.Tablestatuses
            .Select(c => new Tablestatus
            {
                Id = c.Id,
                Statusname = c.Statusname,
            })
            .OrderBy(c => c.Id).ToList();

        tableStatus = new List<Tablestatus>(tableStatus);
        return tableStatus;
    }
    // Get List Of All Sections
    public IEnumerable<SectionNameViewModel> GetSectionList()
    {

        var sections = _context.Sections
                         .Where(c => c.Isdeleted != true) // Exclude deleted categories
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
                        Status = _context.Tablestatuses.FirstOrDefault(s => s.Id == i.Status).Statusname
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        query = query.OrderBy(i => i.TableId);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.Sectionid = sectionid;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
        return model;
    }



    // Add new Section
    public async Task<AuthResponse> AddSection(SectionNameViewModel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var existingsection = _context.Sections.FirstOrDefault(s => s.SectionName.ToLower() == model.SectionName.ToLower() && s.Isdeleted != true);

            if (existingsection != null)
            {
                if (existingsection.Isdeleted == true)
                {
                    existingsection.Isdeleted = false;
                    existingsection.Description = model.Description;
                    existingsection.Createdby = userid;
                    _context.Sections.Update(existingsection);
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
        catch (Exception e)
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
    public async Task<AuthResponse> EditSection(SectionNameViewModel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var existingsectionname = _context.Sections.FirstOrDefault(s => s.SectionName.ToLower() == model.SectionName.ToLower() && s.SectionId != model.SectionId && s.Isdeleted != true);

            if (existingsectionname != null)
            {

                return new AuthResponse
                {
                    Success = false,
                    Message = "Section Already Existed!"
                };
            }

            var existingsection = _context.Sections.FirstOrDefault(s => s.SectionId == model.SectionId);

            if (existingsection == null)
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
            existingsection.Modifieddate = DateTime.Now;


            _context.Sections.Update(existingsection);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Section Updated Succesfully!"
            };
        }
        catch (Exception e)
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
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var existingtablename = _context.Diningtables.FirstOrDefault(t => t.Name.ToLower() == model.Name.ToLower());

            if (existingtablename != null)
            {
                if (existingtablename.Isdeleted == true)
                {
                    existingtablename.Status = model.Status;
                    existingtablename.Capacity = model.Capacity;
                    existingtablename.SectionId = model.SectionId;
                    existingtablename.Isdeleted = false;

                    _context.Diningtables.Update(existingtablename);
                    await _context.SaveChangesAsync();

                    return new AuthResponse
                    {
                        Success = true,
                        Message = "Table Added Succesfully!"
                    };
                }
                else
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Table Already Existed!"
                    };
                }
            }

            var table = new Diningtable
            {
                SectionId = model.SectionId,
                Name = model.Name,
                Capacity = model.Capacity,
                Status = model.Status,
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
        catch (Exception e)
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
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var table = _context.Diningtables.FirstOrDefault(t => t.TableId == model.TableId);

            var existingtablename = _context.Diningtables.FirstOrDefault(t => t.Name.ToLower() == model.Name.ToLower() && t.TableId != model.TableId);

            if (existingtablename != null)
            {
                if (existingtablename.Isdeleted == true)
                {
                    existingtablename.Status = model.Status;
                    existingtablename.Capacity = model.Capacity;
                    existingtablename.SectionId = model.SectionId;

                    _context.Diningtables.Update(existingtablename);
                    await _context.SaveChangesAsync();

                    return new AuthResponse
                    {
                        Success = true,
                        Message = "Table edited Succesfully!"
                    };
                }
                else
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Table Already Existed!"
                    };
                }
            }


            table.SectionId = model.SectionId;
            table.Name = model.Name;
            table.Capacity = model.Capacity;
            table.Status = model.Status;
            table.Modifyiedby = userid;


            _context.Diningtables.Update(table);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Table Updated Succesfully!"
            };
        }
        catch (Exception e)
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

    // Get Table status Id By Name 

    public int GetTableStatusIdByName(string name)
    {
        var tableStatus = _context.Tablestatuses.FirstOrDefault(c => c.Statusname.ToLower() == name.ToLower());
        if (tableStatus != null)
        {
            return tableStatus.Id;
        }
        return 0;
    }

    public async Task<AddTableViewmodel> GetTableDetailById(int id)
    {
        try
        {
            var Tablestatuslist = _context.Tablestatuses.ToList();
            var sectionlist = GetSectionList().ToList();
            if (id == 0)
            {
                var model = new AddTableViewmodel();
                model.Tablestatuses = Tablestatuslist;
                model.Sections = sectionlist;
                return model;
            }
            else
            {
                var data = await _context.Diningtables.FirstOrDefaultAsync(t => t.TableId == id && t.Isdeleted != true);
                return new AddTableViewmodel
                {
                    TableId = id,
                    Capacity = data.Capacity,
                    Name = data.Name,
                    SectionId = data.SectionId ?? 0,
                    Status = data.Status,
                    Tablestatuses = Tablestatuslist,
                    Sections = sectionlist
                };
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }
    public async Task<SectionNameViewModel> GetSectionDetailById(int id)
    {
        try
        {
            if (id == 0)
            {
                return new SectionNameViewModel();
            }
            else
            {
                var data = await _context.Sections.FirstOrDefaultAsync(i => i.SectionId == id && i.Isdeleted != true);
                return new SectionNameViewModel
                {
                    SectionId = id,
                    SectionName = data.SectionName,
                    Description = data.Description
                };
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<int> GetBusyTableCountOfSection(int sectionid = 0)
    {
        int ct = await _context.Diningtables.Where(t => t.Status != 1 && t.SectionId == sectionid && t.Isdeleted != true).CountAsync();
        return ct;
    }
}
