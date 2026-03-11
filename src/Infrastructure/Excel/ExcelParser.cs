using ClosedXML.Excel;
using ExcelDashboard.Application.Contracts.Dashboard;

namespace ExcelDashboard.EntityFrameworkCore.Excel;

public class ExcelParser : IExcelParser
{
    public IReadOnlyList<ParsedExcelRow> Parse(byte[] content, string currency)
    {
        var currencyCode = string.IsNullOrWhiteSpace(currency) || currency.Length != 3
            ? "USD"
            : currency.Trim().ToUpperInvariant();

        using var stream = new MemoryStream(content);
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheets.FirstOrDefault();
        if (sheet == null) return Array.Empty<ParsedExcelRow>();

        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
        if (lastRow < 2) return Array.Empty<ParsedExcelRow>();

        var list = new List<ParsedExcelRow>();
        for (var row = 2; row <= lastRow; row++)
        {
            var categoryCell = sheet.Cell(row, 1).GetString().Trim();
            if (string.IsNullOrEmpty(categoryCell)) continue;
            var amountCell = sheet.Cell(row, 2);
            var dateCell = sheet.Cell(row, 3);
            if (!amountCell.TryGetValue(out double amountNum)) continue;
            if (!dateCell.TryGetValue(out DateTime dateValue)) continue;
            list.Add(new ParsedExcelRow(categoryCell, (decimal)amountNum, currencyCode, dateValue));
        }

        return list;
    }
}
