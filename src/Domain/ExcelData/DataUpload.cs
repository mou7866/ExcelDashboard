using Volo.Abp.Domain.Entities.Auditing;
using ExcelDashboard.Domain.Common;

namespace ExcelDashboard.Domain.ExcelData;

public class DataUpload : FullAuditedAggregateRoot<Guid>
{
    public DataUpload(Guid id, Guid userId, string fileName)
        : base(id)
    {
        if (userId == Guid.Empty) throw new DomainException("Upload.UserRequired");
        if (string.IsNullOrWhiteSpace(fileName)) throw new DomainException("Upload.FileNameRequired");
        UserId = userId;
        FileName = fileName;
        Rows = new List<DataRow>();
    }

    protected DataUpload()
    {
        FileName = string.Empty;
        Rows = new List<DataRow>();
    }

    public Guid UserId { get; private set; }

    public string FileName { get; private set; }

    public ICollection<DataRow> Rows { get; private set; }

    public void AddRow(string category, Money amount, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(category)) throw new DomainException("Row.CategoryRequired");
        if (date == DateTime.MinValue) throw new DomainException("Row.DateRequired");
        var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        var row = new DataRow(Guid.NewGuid(), Id, category, amount, utcDate);
        Rows.Add(row);
    }
}

public class DataRow : FullAuditedEntity<Guid>
{
    public DataRow(Guid id, Guid uploadId, string category, Money amount, DateTime date)
        : base(id)
    {
        if (uploadId == Guid.Empty) throw new DomainException("Row.UploadRequired");
        if (string.IsNullOrWhiteSpace(category)) throw new DomainException("Row.CategoryRequired");
        UploadId = uploadId;
        Category = category;
        Amount = amount;
        Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }

    protected DataRow()
    {
        Category = string.Empty;
        Amount = new Money(0, "USD");
    }

    public Guid UploadId { get; private set; }

    public string Category { get; private set; }

    public Money Amount { get; private set; }

    public DateTime Date { get; private set; }
}

