using ExcelDashboard.Domain.ExcelData;

namespace ExcelDashboard.Domain.Repositories;

public interface IDataUploadRepository
{
    Task InsertAsync(DataUpload upload, CancellationToken cancellationToken = default);
}
