using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ExcelDashboard.Infrastructure.Persistence;

public sealed class ExcelDashboardDbContextFactory : IDesignTimeDbContextFactory<ExcelDashboardDbContext>
{
    public ExcelDashboardDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ExcelDashboardDbContext>();
        var connectionString = "Host=localhost;Port=5432;Database=ExcelDashboard;Username=excel;Password=excel";
        builder.UseNpgsql(connectionString);
        return new ExcelDashboardDbContext(builder.Options);
    }
}

