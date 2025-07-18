using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers;
[ApiController]
[Route("api/reports/assets")]
public class AssetReportController : ControllerBase
{
    private readonly AssetReportService _assetService;

    public AssetReportController(AssetReportService assetService)
    {
        _assetService = assetService;
    }

    [HttpPost("filter")]
    public async Task<IActionResult> GetReport([FromBody] AssetReportRequest request)
    {
        var results = await _assetService.GetFilteredReportAsync(request);
        return Ok(results);
    }
}
