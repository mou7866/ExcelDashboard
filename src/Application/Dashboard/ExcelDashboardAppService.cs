using ExcelDashboard.Application.Contracts.Dashboard;
using ExcelDashboard.Domain.Common;
using ExcelDashboard.Domain.ExcelData;
using ExcelDashboard.Domain.Repositories;
using Volo.Abp.Application.Dtos;

namespace ExcelDashboard.Application.Dashboard;

public class ExcelDashboardAppService : IExcelDashboardAppService
{
    private readonly IDataUploadRepository _uploadRepository;
    private readonly IDataRowRepository _rowRepository;
    private readonly IExcelParser _excelParser;

    public ExcelDashboardAppService(
        IDataUploadRepository uploadRepository,
        IDataRowRepository rowRepository,
        IExcelParser excelParser)
    {
        _uploadRepository = uploadRepository;
        _rowRepository = rowRepository;
        _excelParser = excelParser;
    }

    public async Task UploadAsync(Guid userId, byte[] content, string fileName, string currency)
    {
        if (userId == Guid.Empty) throw new UnauthorizedAccessException();
        if (content == null || content.Length == 0) throw new DomainException("File.Empty");
        if (string.IsNullOrWhiteSpace(fileName) || !fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new DomainException("File.Extension");

        var currencyCode = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();
        if (currencyCode.Length != 3) currencyCode = "USD";

        var rows = _excelParser.Parse(content, currencyCode);
        if (rows.Count == 0) throw new DomainException("File.Empty");

        var upload = new DataUpload(Guid.NewGuid(), userId, fileName);
        foreach (var r in rows)
        {
            var amount = new Money(r.Amount, r.Currency);
            upload.AddRow(r.Category, amount, r.Date);
        }

        await _uploadRepository.InsertAsync(upload);
    }

    public async Task<PagedResultDto<DashboardRowDto>> GetMyRowsAsync(Guid userId, PagedAndSortedResultRequestDto input)
    {
        if (userId == Guid.Empty) throw new UnauthorizedAccessException();

        var (totalCount, list) = await _rowRepository.GetByUserIdPagedAsync(userId, input.SkipCount, input.MaxResultCount);

        var items = list.Select(r => new DashboardRowDto
        {
            Category = r.Category,
            Amount = r.Amount.Value,
            Currency = r.Amount.Currency,
            Date = r.Date
        }).ToList();

        return new PagedResultDto<DashboardRowDto>(totalCount, items);
    }

    public async Task<DashboardSummaryDto> GetMySummaryAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new UnauthorizedAccessException();

        var list = await _rowRepository.GetAllByUserIdForSummaryAsync(userId);

        var byCategory = list
            .GroupBy(x => x.Category)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount.Value));

        var items = list
            .OrderByDescending(x => x.Date)
            .Take(200)
            .Select(x => new DashboardRowDto
            {
                Category = x.Category,
                Amount = x.Amount.Value,
                Currency = x.Amount.Currency,
                Date = x.Date
            })
            .ToList();

        return new DashboardSummaryDto
        {
            Items = items,
            AmountByCategory = byCategory
        };
    }
}
