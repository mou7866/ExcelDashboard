using ExcelDashboard.Domain.ExcelData;
using ExcelDashboard.Domain.Repositories;
using ExcelDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExcelDashboard.EntityFrameworkCore.Repositories;

public class EfDataUploadRepository : IDataUploadRepository
{
    private readonly ExcelDashboardDbContext _dbContext;

    public EfDataUploadRepository(ExcelDashboardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(DataUpload upload, CancellationToken cancellationToken = default)
    {
        await _dbContext.DataUploads.AddAsync(upload, cancellationToken);
        foreach (var row in upload.Rows)
            await _dbContext.DataRows.AddAsync(row, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
