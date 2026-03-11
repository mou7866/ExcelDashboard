using ExcelDashboard.Domain.ExcelData;

namespace ExcelDashboard.Domain.Repositories;

public interface IDataRowRepository
{
    Task<(int TotalCount, IReadOnlyList<DataRow> Items)> GetByUserIdPagedAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DataRow>> GetAllByUserIdForSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}
