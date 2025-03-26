using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DAL.ViewModels;
using OfficeOpenXml;

public class ExcelExportService
{
    public byte[] ExportOrdersToExcel(IEnumerable<OrderViewModel> orderlist,string searchkeyword="",string status="",string timeframe="")
    {
        List<OrderViewModel> orders = orderlist.ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Orders");

            var imagePath = "D:\\pizzashop_3tier\\pizzashop.presentation\\wwwroot\\images\\logos\\pizzashop_logo.png"; // Change this to your image path
            var image = worksheet.AddPicture(imagePath)
                                 .MoveTo(worksheet.Cell(2, 15)) // Move image inside cell B2
                                 .WithSize(100, 80); // Set image size (width, height)

            int row = 9; // Start Header at Row 9

            //  **Set Header Row**
            worksheet.Cell(row, 1).Value = "ID";
            worksheet.Cell(row, 2).Value = "Date";
            worksheet.Cell(row, 5).Value = "Customer";
            worksheet.Cell(row, 8).Value = "Status";
            worksheet.Cell(row, 10).Value = "Payment Mode";
            worksheet.Cell(row, 12).Value = "Rating";
            worksheet.Cell(row, 14).Value = "Total Amount";

            var dateRange = worksheet.Range(row, 2, row, 4).Merge();
            var customerRange = worksheet.Range(row, 5, row, 7).Merge();
            var statusRange = worksheet.Range(row, 8, row, 9).Merge();
            var paymentModeRange = worksheet.Range(row, 10, row, 11).Merge();
            var RatingRange = worksheet.Range(row, 12, row, 13).Merge();
            var amountRange = worksheet.Range(row, 14, row, 15).Merge();

            //  **Merge Cells for a Status Header (Optional)**
            var StatusHeaderRange = worksheet.Range("A2:B3").Merge();
            StatusHeaderRange.Value = "Status :";
            StatusHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            StatusHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            StatusHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            StatusHeaderRange.Style.Font.FontColor = XLColor.White;
            StatusHeaderRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;

            var StatusTitleRange = worksheet.Range("C2:F3").Merge();
            StatusTitleRange.Value = !string.IsNullOrEmpty(status)?status:"All Status";
            StatusTitleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            StatusTitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            StatusTitleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            StatusTitleRange.Style.Font.FontColor = XLColor.Black;
            StatusTitleRange.Style.Fill.BackgroundColor = XLColor.White;


            //  **Merge Cells for a Search Text (Optional)**
            var searchHeaderRange = worksheet.Range("H2:I3").Merge();
            searchHeaderRange.Value = "Search Keyword";
            searchHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            searchHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            searchHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            searchHeaderRange.Style.Font.FontColor = XLColor.White;
            searchHeaderRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;

            var searchTitleRange = worksheet.Range("J2:M3").Merge();
            searchTitleRange.Value = searchkeyword;
            searchTitleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            searchTitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            searchTitleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            searchTitleRange.Style.Font.FontColor = XLColor.Black;
            searchTitleRange.Style.Fill.BackgroundColor = XLColor.White;

            //  **Merge Cells for a Date Range (Optional)**
            var DateHeaderRange = worksheet.Range("A5:B6").Merge();
            DateHeaderRange.Value = "Date :";
            DateHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            DateHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            DateHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            DateHeaderRange.Style.Font.FontColor = XLColor.White;
            DateHeaderRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;

            var DateTitleRange = worksheet.Range("C5:F6").Merge();
            DateTitleRange.Value = timeframe;
            DateTitleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            DateTitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            DateTitleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            DateTitleRange.Style.Font.FontColor = XLColor.Black;
            DateTitleRange.Style.Fill.BackgroundColor = XLColor.White;

            //  **No. Of Record **
            var RecordHeaderRange = worksheet.Range("H5:I6").Merge();
            RecordHeaderRange.Value = "Total Records :";
            RecordHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            RecordHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            RecordHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            RecordHeaderRange.Style.Font.FontColor = XLColor.White;
            RecordHeaderRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;

            var RecordTitleRange = worksheet.Range("J5:M6").Merge();
            RecordTitleRange.Value = orders.Count;
            RecordTitleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            RecordTitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            RecordTitleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            RecordTitleRange.Style.Font.FontColor = XLColor.Black;
            RecordTitleRange.Style.Fill.BackgroundColor = XLColor.White;


            //  **Style Header Row**
            var headerRange = worksheet.Range("A9:O9");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // **Insert Data Rows**
            row++;
            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value = order.OrderId;
                worksheet.Cell(row, 2).Value = order.OrderDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 5).Value = order.CustomerName;
                worksheet.Cell(row, 8).Value = order.OrderStatus;
                worksheet.Cell(row, 10).Value = order.PaymentMode;
                worksheet.Cell(row, 12).Value = order.Rating != null ? order.Rating : "No Rating"; // If null, display "No Rating"
                worksheet.Cell(row, 14).Value = order.TotalAmount;

                worksheet.Range(row, 2, row, 4).Merge();
                worksheet.Range(row, 5, row, 7).Merge();
                worksheet.Range(row, 8, row, 9).Merge();
                worksheet.Range(row, 10, row, 11).Merge();
                worksheet.Range(row, 12, row, 13).Merge();
                worksheet.Range(row, 14, row, 15).Merge();

                // Apply Borders to Each Row
                // worksheet.Range($"A{row}:G{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                var dataRowRange = worksheet.Range($"A{row}:O{row}");
                dataRowRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataRowRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                dataRowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                row++;
            }

            //**Set Fixed Column Width**
            worksheet.Column(1).Width = 10;  // ID
            worksheet.Column(2).Width = 10;  // Date
            worksheet.Column(3).Width = 10;  // Customer
            worksheet.Column(4).Width = 10;  // Status
            worksheet.Column(5).Width = 10;  // Payment Mode
            worksheet.Column(6).Width = 10;  // Rating
            worksheet.Column(7).Width = 10;  // Total Amount

            //  **AutoFit for Better Formatting**
            // worksheet.Columns().AdjustToContents();

            //  **Save File to MemoryStream**
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray(); // Return as byte array
            }
        }
    }
}
