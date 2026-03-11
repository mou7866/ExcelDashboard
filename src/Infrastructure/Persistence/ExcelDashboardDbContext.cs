using System.Reflection;
using ExcelDashboard.Domain.ExcelData;
using Microsoft.EntityFrameworkCore;

namespace ExcelDashboard.Infrastructure.Persistence;

public class ExcelDashboardDbContext : DbContext
{
    public ExcelDashboardDbContext(DbContextOptions<ExcelDashboardDbContext> options)
        : base(options)
    {
    }

    public DbSet<DataUpload> DataUploads { get; set; }

    public DbSet<DataRow> DataRows { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added) continue;
            var e = entry.Entity;
            SetCreationTime(e);
            SetConcurrencyStamp(e);
            if (entry.Metadata.FindProperty("ExtraPropertiesJson") != null)
                entry.Property("ExtraPropertiesJson").CurrentValue = "{}";
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    private static void SetCreationTime(object entity)
    {
        var prop = entity.GetType().GetProperty("CreationTime", BindingFlags.Public | BindingFlags.Instance);
        if (prop?.CanWrite == true)
            prop.SetValue(entity, DateTime.UtcNow);
    }

    private static void SetConcurrencyStamp(object entity)
    {
        var prop = entity.GetType().GetProperty("ConcurrencyStamp", BindingFlags.Public | BindingFlags.Instance);
        if (prop?.CanWrite == true && prop.GetValue(entity) == null)
            prop.SetValue(entity, Guid.NewGuid().ToString("N")[..40]);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<DataUpload>(b =>
        {
            b.ToTable("DataUploads");
            b.HasKey(x => x.Id);
            b.Ignore("ExtraProperties");
            b.Property<string>("ExtraPropertiesJson").HasColumnName("ExtraProperties").HasDefaultValue("{}");
            b.Property(x => x.FileName).IsRequired().HasMaxLength(256);
            b.Property(x => x.UserId).IsRequired();
            b.HasMany(x => x.Rows).WithOne().HasForeignKey(x => x.UploadId);
            b.HasIndex(x => x.UserId);
        });

        builder.Entity<DataRow>(b =>
        {
            b.ToTable("DataUploadRows");
            b.HasKey(x => x.Id);
            b.Ignore("ExtraProperties");
            b.Property(x => x.Category).IsRequired().HasMaxLength(128);
            b.Property(x => x.UploadId).IsRequired();
            b.OwnsOne(x => x.Amount, mb =>
            {
                mb.Property(p => p.Value).HasColumnName("AmountValue").HasColumnType("decimal(18,2)");
                mb.Property(p => p.Currency).HasColumnName("AmountCurrency").HasMaxLength(3);
            });
            b.Property(x => x.Date).IsRequired();
        });
    }
}

