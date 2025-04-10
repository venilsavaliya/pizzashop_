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
                TotalWaitingToken = _context.Waitingtokens.Where(t => t.Sectionid == s.SectionId).Count()

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
                Message = "Token Created Successfully!"
            };
        }
        catch (System.Exception)
        {

            return new AuthResponse
            {
                Success = false,
                Message = "Error Creating Token!"
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
                where token.Isdeleted == false && (sectionid == 0 || token.Sectionid == sectionid)
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
            ).ToListAsync();

            return waitingtokenlist;

        }
        catch (System.Exception)
        {

            return new List<WaitingTokenViewModel>();
        }
    }


}

