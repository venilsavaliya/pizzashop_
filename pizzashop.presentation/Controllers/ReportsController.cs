using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;


public class ReportsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> ExportToPdf()
    {

        return new ViewAsPdf("ExportView")
        {
            FileName = "Report.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
        };
    }
}

