using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using YourNamespace.Services;
using MongoDB.Bson;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetDisposalController : ControllerBase
    {
        private readonly AssetDisposalService _service;

        public AssetDisposalController(AssetDisposalService service)
        {
            _service = service;
        }

        [HttpGet(" AssetDisposalsummary")]
        public async Task<IActionResult> GetSummary([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetSummaryAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        

   
    // GET when clicking from email
[HttpGet("process")]
public async Task<IActionResult> ApproveFromEmail([FromQuery] string id)
{
    if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
        return BadRequest(new { error = "Invalid or missing ID" });

    var (success, message) = await _service.CreateOrApproveDisposalAsync(null, id);
    if (!success)
        return BadRequest(new { error = message });

    return Ok(new { message });
}

// POST when creating a new disposal (from frontend)
[HttpPost("process")]
public async Task<IActionResult> CreateDisposal([FromBody] AssetDisposal disposal)
{
    var (success, message) = await _service.CreateOrApproveDisposalAsync(disposal, null);
    if (!success)
        return BadRequest(new { error = message });

    return Ok(new { message });
}


        [HttpPut("updateassetdisposal{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AssetDisposal updated)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            updated.Id = id;
            await _service.UpdateAsync(id, updated);
            return Ok(new { message = "Disposal updated successfully." });
        }

        [HttpDelete("deleteassetDisposal{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Asset deleted successfully." });
        }
       
 [HttpGet("assets/{disposalId}")]
public async Task<IActionResult> GetAssetsByDisposalId(string disposalId)
{
    var assets = await _service.GetAssetsForDisposal(disposalId);

    if (assets == null || !assets.Any())
        return NotFound("No assets found for the given disposal ID.");

    return Ok(assets);
}
[HttpPost("report")]
public async Task<IActionResult> GetDisposalReport([FromBody] AssetDisposalReportFilterDto filter)
{
    var result = await _service.GetDisposalReportAsync(filter);

    if (result == null || !result.Any())
        return NotFound("No matching asset disposal records found.");

    return Ok(result);
}


    }
}
