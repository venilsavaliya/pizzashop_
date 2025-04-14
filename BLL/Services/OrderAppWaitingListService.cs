using BLL.Interfaces;
using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class OrderAppWaitingListService : IOrderAppWaitingListService
{
    private readonly ApplicationDbContext _context;
    private readonly IOrderAppTableService _OrderAppTable;
    private readonly ICustomerService _customerservice;

    private readonly IUserService _userservices;
    private readonly IHttpContextAccessor _httpContext;

    public OrderAppWaitingListService(ApplicationDbContext context, ICustomerService customerservice, IUserService userservices, IHttpContextAccessor httpContext)
    {
        _context = context;
        _customerservice = customerservice;
        _userservices = userservices;
        _httpContext = httpContext;
    }



    // Get Section List Of Waiting 

    public async Task<WaitingListMainViewModel> GetSectionList()
    {
        try
        {
            var sectionList = await _context.Sections.Select(s => new SectionListWaiting
            {
                SectionId = s.SectionId,
                SectionName = s.SectionName,
                TotalWaitingToken = _context.Waitingtokens.Where(t => t.Sectionid == s.SectionId && t.Completiontime ==null).Count()

            }).ToListAsync();

            var model = new WaitingListMainViewModel
            {
                section = sectionList,
                TotalWaitingToken = _context.Waitingtokens.Where(t => t.Completiontime == null).Count()
            };

            return model;

        }
        catch (System.Exception)
        {
            return new WaitingListMainViewModel();
        }
    }

    // Add waiting token 

    public async Task<AuthResponse> AddWaitingToken(AddEditWaitingTokenViewModel model)
    {
        try
        {
            var customer = model.Customer;

            //this will return id of newly created customer or existing customer
            var customerid = _customerservice.AddCustomer(customer).Result;

            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            // if we get token id than we simply update the token
            if (model.Tokenid != null)
            {
                var existingwaitingtoken = _context.Waitingtokens.FirstOrDefault(t => t.Tokenid == model.Tokenid);
                existingwaitingtoken.Sectionid = model.Sectionid;
                existingwaitingtoken.Customerid = customerid;
                existingwaitingtoken.Totalperson = model.Customer.TotalPerson;
            }
            // else we create new token
            else
            {
                var waitingtoken = new Waitingtoken
                {
                    Customerid = customerid,
                    Totalperson = model.Customer.TotalPerson,
                    Sectionid = model.Sectionid,
                    Createdby = userid
                };

                _context.Waitingtokens.Add(waitingtoken);
            }


            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = model.Tokenid != null ? "Token Updated Successfully!" : "Token Created Succesfully",
            };
        }
        catch (System.Exception)
        {

            return new AuthResponse
            {
                Success = false,
                Message = "Some Error Occured!"
            };
        }
    }

    // // Get Waiting Token List 

    public async Task<List<WaitingTokenViewModel>> GetWaitingTokenList(int sectionid = 0)
    {
        try
        {
            var waitingtokenlist = await (
                from token in _context.Waitingtokens
                join customer in _context.Customers on token.Customerid equals customer.CustomerId
                where token.Isdeleted == false && (sectionid == 0 || token.Sectionid == sectionid) && token.Completiontime ==null
                select new WaitingTokenViewModel
                {
                    Tokenid = token.Tokenid,
                    sectionid = token.Sectionid,
                    Createdat = token.Createdat,
                    Name = customer.Name,
                    Email = customer.Email,
                    Mobile = customer.Mobile,
                    Totalperson = token.Totalperson
                }
            ).OrderBy(w => w.Tokenid).ToListAsync();

            return waitingtokenlist;

        }
        catch (System.Exception)
        {

            return new List<WaitingTokenViewModel>();
        }
    }


    // delete waiting token
    public async Task<AuthResponse> DeleteWaitingToken(int TokenId)
    {
        try
        {
            var existingtoken = await _context.Waitingtokens.FirstOrDefaultAsync(t => t.Tokenid == TokenId);
            if (existingtoken != null)
            {
                existingtoken.Isdeleted = true;

                await _context.SaveChangesAsync();
                return new AuthResponse
                {
                    Message = "Token Deleted Succefully!",
                    Success = true
                };
            }
            else
            {
                return new AuthResponse
                {
                    Message = "Error Occured!",
                    Success = false
                };
            }
        }
        catch (System.Exception)
        {
            return new AuthResponse
            {
                Message = "Error Occured!",
                Success = false
            };
            throw;
        }
    }


    // get available table list

    public async Task<List<TableViewModel>> GetAvailableTableList(int SectionId)
    {
        try
        {   var availablestatus = _context.Tablestatuses.FirstOrDefault(s=>s.Statusname==Constants.Available)!.Id;
            var tables = await _context.Diningtables.Where(t=> t.SectionId==SectionId && t.Status==availablestatus && t.Isdeleted!=true).Select(i=> new TableViewModel{
                TableId = i.TableId,
                Name = i.Name,
                Capacity = i.Capacity
            }).ToListAsync();

            return tables;
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
}   

