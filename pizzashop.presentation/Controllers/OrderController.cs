using BLL.Interfaces;
using BLL.Services;
using DAL.ViewModels;
using DAL.Constants;
using BLL.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;

namespace pizzashop.presentation.Controllers;

public class OrderController : BaseController
{
    private readonly IOrderService _orderservice;

    private readonly ExcelExportService _excelExportService;

    public OrderController(IJwtService jwtService, IUserService userService, IAdminService adminservice, IAuthorizationService authservice, IOrderService orderservice, ExcelExportService excelExportService) : base(jwtService, userService, adminservice, authservice)
    {
        _orderservice = orderservice;
        _excelExportService = excelExportService;
    }



    [AuthorizePermission(PermissionName.Orders, ActionPermission.CanView)]
    public IActionResult Index()
    {
        ViewBag.active = "Order";
        return View();
    }

    [AuthorizePermission(PermissionName.Orders, ActionPermission.CanView)]
    public async Task<IActionResult> OrderDetail(int id)
    {
        ViewBag.active = "Order";
        var model = await _orderservice.GetOrderDetailByOrderId(id);
        if(model == null)
        {
            return View("Index");
        }
        return View(model);
    }

    // Return Partial View Of Filtered Order Table List
    [AuthorizePermission(PermissionName.Orders, ActionPermission.CanView)]
    public async Task<IActionResult> GetOrderList(string sortColumn = "", string sortOrder = "", int pageNumber = 1, int pageSize = 5, string searchKeyword = "", string status = "", string startDate = null, string endDate = null)
    {

        if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
        {
            var model = await _orderservice.GetOrderList(sortColumn, sortOrder, pageNumber, pageSize, searchKeyword, status, DateTime.Parse(startDate), DateTime.Parse(endDate));
            return PartialView("~/Views/Order/_OrderList.cshtml", model);
        }
        else
        {
            var model = await _orderservice.GetOrderList(sortColumn, sortOrder, pageNumber, pageSize, searchKeyword, status);
            return PartialView("~/Views/Order/_OrderList.cshtml", model);
        }

    }

    // Export The List Of Orders
    [AuthorizePermission(PermissionName.Orders, ActionPermission.CanView)]
    public async Task<IActionResult> ExportOrders(string searchKeyword = "", string status = "", string startDate = null, string endDate = null,string timeframe = "")
    {

        if (!startDate.IsNullOrEmpty() && !endDate.IsNullOrEmpty())
        {
            var orders = await _orderservice.GetOrderListForExport(searchKeyword, status, DateTime.Parse(startDate), DateTime.Parse(endDate));
           
            byte[] fileContents = _excelExportService.ExportOrdersToExcel(orders,searchKeyword,status,timeframe);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
        }
        else
        {
            var orders = await _orderservice.GetOrderListForExport(searchKeyword, status);

            byte[] fileContents = _excelExportService.ExportOrdersToExcel(orders,searchKeyword,status,timeframe);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
        }

    }

    public async Task<IActionResult> ExportToPdf(int id)
    {
        var model = await _orderservice.GetOrderDetailByOrderId(id);

        return new ViewAsPdf("ExportView",model)
        {
            FileName = "Report.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
        };
    }
}
