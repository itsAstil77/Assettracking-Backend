using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using YourNamespace.Services;
using MongoDB.Bson;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetMovementController : ControllerBase
    {
        private readonly AssetMovementService _service;

        public AssetMovementController(AssetMovementService service)
        {
            _service = service;
        }

        [HttpGet(" Assetmovementsummary")]
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssetMovement movement)
        {
            await _service.CreateAsync(movement);
            return Ok(new { message = "Asset moved successfully." });
        }

        [HttpPut("updateassetmovement{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AssetMovement updated)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            updated.Id = id;
            await _service.UpdateAsync(id, updated);
            return Ok(new { message = "Movement updated successfully." });
        }

        [HttpDelete("deleteassetmovement{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "Movement deleted successfully." });
        }
        [HttpPost("process")]
        public async Task<IActionResult> CreateDisposal([FromBody] AssetMovement disposal)
        {
            var (success, message) = await _service.CreateOrApproveDisposalAsync(disposal, null);
            if (!success)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }
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
    }
}
