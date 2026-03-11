using Volo.Abp.Application.Dtos;

namespace ExcelDashboard.Application.Contracts.Dashboard;

public interface IExcelDashboardAppService
{
    Task UploadAsync(Guid userId, byte[] content, string fileName, string currency);

    Task<PagedResultDto<DashboardRowDto>> GetMyRowsAsync(Guid userId, PagedAndSortedResultRequestDto input);

    Task<DashboardSummaryDto> GetMySummaryAsync(Guid userId);
}

