using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace pizzashop.presentation.Controllers;

public class CustomerController : BaseController
{
    private readonly ICustomerService _customerservice;

    private readonly ExcelExportService _excelExportService;

    public CustomerController(IJwtService jwtService, IUserService userService, IAdminService adminservice, IAuthorizationService authservice, ICustomerService customerservice, ExcelExportService excelExportService) : base(jwtService, userService, adminservice, authservice)
    {
        _customerservice = customerservice;
        _excelExportService = excelExportService;
    }


    [AuthorizePermission(PermissionName.Customers, ActionPermission.CanView)]
    public IActionResult Index()
    {
        ViewBag.active = "Customer";
        return View();
    }

    // Return Partial View Of Filtered Order Table List
    [AuthorizePermission(PermissionName.Customers, ActionPermission.CanView)]
    public async Task<IActionResult> GetCustomerList(string sortColumn = "", string sortOrder = "", int pageNumber = 1, int pageSize = 5, string searchKeyword = "", string startDate = null, string endDate = null)
    {

        if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
        {
            var model = await _customerservice.GetCustomerList(sortColumn, sortOrder, pageNumber, pageSize, searchKeyword, DateTime.Parse(startDate), DateTime.Parse(endDate));
            return PartialView("~/Views/Customer/_CustomerList.cshtml", model);
        }
        else
        {
            var model = await _customerservice.GetCustomerList(sortColumn, sortOrder, pageNumber, pageSize, searchKeyword);
            return PartialView("~/Views/Customer/_CustomerList.cshtml", model);
        }

    }

    // Export The List Of Orders
    [AuthorizePermission(PermissionName.Customers, ActionPermission.CanView)]
    public async Task<IActionResult> ExportCustomers(string searchKeyword = "", string startDate = null, string endDate = null,string timeframe = "")
    {

        if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
        {
            var customers = await _customerservice.GetCustomerListForExport(searchKeyword, DateTime.Parse(startDate), DateTime.Parse(endDate));
           
            byte[] fileContents = _excelExportService.ExportCustomerToExcel(customers,searchKeyword,timeframe);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
        }
        else
        {
            var customers = await _customerservice.GetCustomerListForExport(searchKeyword);

            byte[] fileContents = _excelExportService.ExportCustomerToExcel(customers,searchKeyword,timeframe);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
        }

    }

    // Get Customer History Table Partial View 
     [AuthorizePermission(PermissionName.Customers, ActionPermission.CanView)]
    public async Task<IActionResult> GetCustomerHistory(int customerid)
    {
        var model = await _customerservice.GetCustomerOrderHistory(customerid);
        return PartialView("~/Views/Customer/_CustomerHistory.cshtml", model);
    }
}
