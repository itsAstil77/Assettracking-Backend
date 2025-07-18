using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using YourNamespace.Services;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetCheckinController : ControllerBase
    {
        private readonly AssetCheckinService _service;

        public AssetCheckinController(AssetCheckinService service)
        {
            _service = service;
        }

        [HttpGet(" AssetCheckinsummary")]
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
public async Task<IActionResult> Create([FromBody] AssetCheckin checkin)
{
    await _service.CreateAsync(checkin);
    return Ok(new { message = "Asset check-in completed and asset details updated." });
}

        [HttpPut("updateassetCheckin{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AssetCheckin updated)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            updated.Id = id;
            await _service.UpdateAsync(id, updated);
            return Ok(new { message = "Checkout updated successfully." });
        }

        [HttpDelete("deleteassetCheckin{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "AssetCheckin deleted successfully." });
        }
        [HttpGet("getassetbycustodian")]
        public async Task<IActionResult> getAssetsbycustodian(string name)
        {
            var hello = await _service.getassetbycustodian(name);
            return Ok(hello);
}    }
}
