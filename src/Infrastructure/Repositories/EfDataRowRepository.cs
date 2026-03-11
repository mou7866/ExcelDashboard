using ExcelDashboard.Domain.ExcelData;
using ExcelDashboard.Domain.Repositories;
using ExcelDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExcelDashboard.EntityFrameworkCore.Repositories;

public class EfDataRowRepository : IDataRowRepository
{
    private readonly ExcelDashboardDbContext _dbContext;

    public EfDataRowRepository(ExcelDashboardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int TotalCount, IReadOnlyList<DataRow> Items)> GetByUserIdPagedAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var query = from r in _dbContext.DataRows
                    join u in _dbContext.DataUploads on r.UploadId equals u.Id
                    where u.UserId == userId
                    orderby u.CreationTime descending, r.Date descending
                    select r;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (totalCount, items);
    }

    public async Task<IReadOnlyList<DataRow>> GetAllByUserIdForSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = from r in _dbContext.DataRows
                    join u in _dbContext.DataUploads on r.UploadId equals u.Id
                    where u.UserId == userId
                    select r;

        return await query.ToListAsync(cancellationToken);
    }
}
