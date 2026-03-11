using Volo.Abp.Application.Dtos;

namespace ExcelDashboard.Application.Contracts.Dashboard;

public class DashboardRowDto : EntityDto<Guid>
{
    public required string Category { get; set; }

    public decimal Amount { get; set; }

    public required string Currency { get; set; }

    public DateTime Date { get; set; }
}

public class DashboardSummaryDto
{
    public required IReadOnlyList<DashboardRowDto> Items { get; set; }

    public required IReadOnlyDictionary<string, decimal> AmountByCategory { get; set; }
}

