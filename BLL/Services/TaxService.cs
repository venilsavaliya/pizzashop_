namespace BLL.Services;

using BLL.Helper;
using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;

public class TaxService : ITaxService
{
    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContext;

    private readonly IJwtService _jwtservices;

    private readonly IUserService _userservices;

    public TaxService(ApplicationDbContext dbcontext, IHttpContextAccessor httpContext, IUserService userService, IJwtService jwtservices)
    {
        _context = dbcontext;
        _httpContext = httpContext;
        _userservices = userService;
        _jwtservices = jwtservices;
    }

    public TaxListPaginationViewModel GetTaxList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        TaxListPaginationViewModel model = new() { Page = new() };
        searchKeyword = searchKeyword.ToLower();

        var query = from i in _context.Taxes
                    where i.Isdeleted != true
                    select new TaxViewModel
                    {
                        TaxId = i.TaxId,
                        TaxName = i.TaxName,
                        Type = i.Type,
                        TaxAmount = i.TaxAmount,
                        Isenable = i.Isenable ?? false,
                        Isdefault = i.Isdefault
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.TaxName.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        query = query.OrderBy(i => i.TaxName);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
        return model;
    }


    // Get Tax Detail By Id
    public TaxViewModel GetTaxDetailById(int id)
    {
        try
        {
            var data = _context.Taxes.FirstOrDefault(i => i.TaxId == id);
            return new TaxViewModel
            {
                TaxId = data.TaxId,
                TaxAmount = data.TaxAmount,
                TaxName = data.TaxName,
                Isdefault = data.Isdefault,
                Isenable = data.Isenable ?? false
            };
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    // Add Tax
    public async Task<AuthResponse> AddTax(TaxViewModel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var existingtax = _context.Taxes.FirstOrDefault(i => i.TaxName.ToLower() == model.TaxName.ToLower() && i.Isdeleted == false);

            if (existingtax != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Tax Name Already Existed!"
                };
            }

            var tax = new Taxis
            {
                TaxName = model.TaxName,
                Type = model.Type,
                Isenable = model.Isenable,
                Isdefault = model.Isdefault,
                TaxAmount = model.TaxAmount,
                Createdby = userid
            };

            _context.Taxes.Add(tax);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Tax Added Succesfully!"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in Add Tax: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Adding Tax!"
            };
        }
    }


    // Edit Tax
    public async Task<AuthResponse> EditTax(TaxViewModel model)
    {
        try
        {

            var existingtaxname = _context.Taxes.FirstOrDefault(i => i.TaxName.ToLower() == model.TaxName.ToLower()&& i.TaxId != model.TaxId && i.Isdeleted == false);

            if (existingtaxname != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Tax Name Already Existed!"
                };
            }

            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            var existingtax = _context.Taxes.FirstOrDefault(i => i.TaxId == model.TaxId);


            existingtax.TaxName = model.TaxName;
            existingtax.Type = model.Type;
            existingtax.Isenable = model.Isenable;
            existingtax.Isdefault = model.Isdefault;
            existingtax.TaxAmount = model.TaxAmount;
            existingtax.Modifyiedby = userid;


            _context.Taxes.Update(existingtax);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Tax Edited Succesfully!"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in Edit Tax: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Editing Tax!"
            };
        }
    }

    // Delete single Tax
    public async Task<AuthResponse> DeleteTax(int id)
    {
        var item = _context.Taxes.FirstOrDefault(c => c.TaxId == id);

        if (item != null)
        {
            item.Isdeleted = true;

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Tax Deleted Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "can't delete Tax"

            };
        }
    }




}
