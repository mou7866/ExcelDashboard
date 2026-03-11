using System.Security.Claims;
using ExcelDashboard.Application.Contracts.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace ExcelDashboard.Host.Controllers;

[Route("api/excel-dashboard")]
[ApiController]
public class ExcelDashboardController : ControllerBase
{
    readonly IExcelDashboardAppService appService;

    public ExcelDashboardController(IExcelDashboardAppService appService)
    {
        this.appService = appService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string currency)
    {
        if (file == null) return BadRequest();
        var userId = GetUserId();
        await using var stream = file.OpenReadStream();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        await appService.UploadAsync(userId, memory.ToArray(), file.FileName, currency);
        return NoContent();
    }

    [HttpGet("rows")]
    public Task<PagedResultDto<DashboardRowDto>> GetRowsAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        var userId = GetUserId();
        return appService.GetMyRowsAsync(userId, input);
    }

    [HttpGet("summary")]
    public Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var userId = GetUserId();
        return appService.GetMySummaryAsync(userId);
    }

    Guid GetUserId()
    {
        // Temporary single-user implementation until auth is wired
        return Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    }
}

