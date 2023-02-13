using Microsoft.AspNetCore.Mvc;
using ExcelExplore.Models;
using ClosedXML.Excel;


namespace ExcelExplore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {

        return View();
    }
    public IActionResult SampleFile(){
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");

        // Add headers
        worksheet.Cell("A1").Value = "Id";
        worksheet.Cell("B1").Value = "Name";
        worksheet.Cell("C1").Value = "Age";
        worksheet.Cell("D1").Value = "Address";
        worksheet.Cell("E1").Value = "BirthDate";

        // Style the headers
        var headerRange = worksheet.Range("A1:E1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Flush();
        memoryStream.Position = 0;

        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data.xlsx");
    }
    [HttpGet]
    public IActionResult Export()
    {
        var data = new List<Student>
        {
            new Student { Id = 1, Name = "John Doe", Age = 30, Address = "123 Main St", BirthDate = new DateTime(1993, 1, 1) },
            new Student { Id = 2, Name = "Jane Doe", Age = 25, Address = "456 Main St", BirthDate = new DateTime(1998, 2, 2) },
            new Student { Id = 3, Name = "Tom Smith", Age = 35, Address = "789 Main St", BirthDate = new DateTime(1988, 3, 3) },
            new Student { Id = 4, Name = "Samantha Brown", Age = 40, Address = "246 Main St", BirthDate = new DateTime(1983, 4, 4) },
            new Student { Id = 5, Name = "Michael Johnson", Age = 45, Address = "369 Main St", BirthDate = new DateTime(1978, 5, 5) }
        };

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Data");

        worksheet.Cell(1, 1).Value = "Id";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Age";
        worksheet.Cell(1, 4).Value = "Address";
        worksheet.Cell(1, 5).Value = "BirthDate";

        // Style the headers
        var headerRange = worksheet.Range("A1:E1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (var i = 0; i < data.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = data[i].Id;
            worksheet.Cell(i + 2, 2).Value = data[i].Name;
            worksheet.Cell(i + 2, 3).Value = data[i].Age;
            worksheet.Cell(i + 2, 4).Value = data[i].Address;
            worksheet.Cell(i + 2, 5).Value = data[i].BirthDate;
        }

        var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Flush();
        memoryStream.Position = 0;

        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data.xlsx");
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("File", "Please upload a non-empty file.");
            _logger.LogError("somthing went to wrong");
            return View();
        }

        using (MemoryStream stream = new MemoryStream())
        {
            file.CopyToAsync(stream);
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var firstRowUsed = worksheet.FirstRowUsed();
                var firstCellUsed = firstRowUsed.FirstCellUsed();
                var lastCellUsed = firstRowUsed.LastCellUsed();

                for (var row = firstRowUsed.RowNumber() + 1; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    var id = worksheet.Cell(row, firstCellUsed.Address.ColumnNumber).GetValue<int>();
                    var name = worksheet.Cell(row, firstCellUsed.Address.ColumnNumber + 1).GetValue<string>();
                    var age = worksheet.Cell(row, firstCellUsed.Address.ColumnNumber + 2).GetValue<int>();
                    var address = worksheet.Cell(row, firstCellUsed.Address.ColumnNumber + 3).GetValue<string>();
                    var birthDate = worksheet.Cell(row, firstCellUsed.Address.ColumnNumber + 4).GetValue<DateTime>();

                    var data = new Student
                    {
                        Id = id,
                        Name = name,
                        Age = age,
                        Address = address,
                        BirthDate = birthDate
                    };

                   // _context.Add(data);
                }
                _logger.LogInformation("Data Save Successfully");
               // _context.SaveChanges();
            }
        }

        return RedirectToAction("Index");
    }
}


