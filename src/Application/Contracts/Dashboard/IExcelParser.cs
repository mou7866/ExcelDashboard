namespace ExcelDashboard.Application.Contracts.Dashboard;

public record ParsedExcelRow(string Category, decimal Amount, string Currency, DateTime Date);

public interface IExcelParser
{
    IReadOnlyList<ParsedExcelRow> Parse(byte[] content, string currency);
}
