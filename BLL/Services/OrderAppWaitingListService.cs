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
                TotalWaitingToken = _context.Waitingtokens.Where(t => t.Sectionid == s.SectionId && t.Completiontime == null && t.Isdeleted != true).Count()

            }).ToListAsync();

            var model = new WaitingListMainViewModel
            {
                section = sectionList,
                TotalWaitingToken = _context.Waitingtokens.Where(t => t.Completiontime == null && t.Isdeleted !=true).Count()
            };

            return model;

        }
        catch (System.Exception)
        {
            return new WaitingListMainViewModel();
        }
    }

    // Get data For Add Edit Waiting Token

    public async Task<AddEditWaitingTokenViewModel> GetAddEditWaitingTokenDetail(int id = 0)
    {
        var sectionlist = _context.Sections.Select(i => new SectionNameViewModel
        {
            SectionId = i.SectionId,
            SectionName = i.SectionName,
            Description = i.Description
        }).ToList();

        var model = new AddEditWaitingTokenViewModel();

        model.Sections = sectionlist;

        if (id != 0)
        {
            var waitingtoken = _context.Waitingtokens.FirstOrDefault(i => i.Tokenid == id);
            if (waitingtoken != null)
            {
                model.Tokenid = id;
                model.Sectionid = waitingtoken.Sectionid;
                model.Customer = _context.Customers.Select(i => new AddCustomerViewModel
                {
                    CustomerId = i.CustomerId,
                    Email = i.Email,
                    Mobile = i.Mobile,
                    Name = i.Name,
                    TotalPerson = i.Totalperson ?? 0,
                    TotalVisit = i.TotalVisit,
                }).FirstOrDefault(j => j.CustomerId == waitingtoken.Customerid) ?? new AddCustomerViewModel();
            }

        }
        return model;
    }

    // Add waiting token 

    public async Task<AuthResponse> AddWaitingToken(AddEditWaitingTokenViewModel model)
    {
        try
        {
            var customer = model.Customer;

            //this will return id of newly created customer or existing customer
            var customerid = await _customerservice.AddCustomer(customer);

            var token = _httpContext.HttpContext.Request.Cookies["jwt"];
            var userid = _userservices.GetUserIdfromToken(token);

            // if we get token id than we simply update the token
            if (model.Tokenid != null)
            {


                var existingwaitingtoken = _context.Waitingtokens.FirstOrDefault(t => t.Tokenid == model.Tokenid);
                existingwaitingtoken.Sectionid = model.Sectionid;
                existingwaitingtoken.Customerid = customerid;
                existingwaitingtoken.Totalperson = model.Customer.TotalPerson;
                existingwaitingtoken.Modifiedby = userid;
            }
            // else we create new token
            else
            {
                // first we check if there is already running token exist with same email
                var existingtoken = _context.Waitingtokens.FirstOrDefault(t => t.Customerid == customerid && t.Completiontime == null);

                if (existingtoken != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Waiting Token Already Existed!",
                    };
                }
                else
                {

                    var isCustomerInShop = _context.Diningtables.FirstOrDefault(t => t.Customerid == customerid);

                    if (isCustomerInShop != null)
                    {
                        return new AuthResponse
                        {
                            Success = false,
                            Message = "Customer is Already Assigned a Table!",
                        };
                    }
                }

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
                where token.Isdeleted == false && (sectionid == 0 || token.Sectionid == sectionid) && token.Completiontime == null
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
        {
            var availablestatus = _context.Tablestatuses.FirstOrDefault(s => s.Statusname == Constants.Available)!.Id;
            var tables = await _context.Diningtables.Where(t => t.SectionId == SectionId && t.Status == availablestatus && t.Isdeleted != true).Select(i => new TableViewModel
            {
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

    // Get order Id Od Table 

    public int GetOrderIdOfTable (int tableid)
    {
        try
        {
            if(tableid==0)
            {
                return 0;
            }

            return _context.Tableorders.First(i=>i.TableId==tableid).OrderId??0;
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
}

