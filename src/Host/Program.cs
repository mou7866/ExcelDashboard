using ExcelDashboard.Application.Contracts.Dashboard;
using ExcelDashboard.Application.Dashboard;
using ExcelDashboard.Domain.Repositories;
using ExcelDashboard.EntityFrameworkCore.Excel;
using ExcelDashboard.EntityFrameworkCore.Repositories;
using ExcelDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Host=localhost;Port=5432;Database=ExcelDashboard;Username=excel;Password=excel";
builder.Services.AddDbContext<ExcelDashboardDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IDataUploadRepository, EfDataUploadRepository>();
builder.Services.AddScoped<IDataRowRepository, EfDataRowRepository>();
builder.Services.AddScoped<IExcelParser, ExcelParser>();
builder.Services.AddScoped<IExcelDashboardAppService, ExcelDashboardAppService>();

var app = builder.Build();



    app.UseSwagger();
    app.UseSwaggerUI();


app.UseRouting();
app.UseCors();

app.MapControllers();

app.Run();

