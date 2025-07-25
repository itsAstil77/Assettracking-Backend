using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using YourNamespace.Services;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetCheckoutController : ControllerBase
    {
        private readonly AssetCheckoutService _service;

        public AssetCheckoutController(AssetCheckoutService service)
        {
            _service = service;
        }

        [HttpGet(" AssetCheckoutsummary")]
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
public async Task<IActionResult> Create([FromBody] AssetCheckout checkout)
{
    await _service.createAsyncAssetcheckout(checkout);
    return Ok(new { message = "Asset check-out completed and asset details updated." });
}

        [HttpPut("updateassetCheckout{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AssetCheckout updated)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            updated.Id = id;
            await _service.UpdateAsync(id, updated);
            return Ok(new { message = "Checkout updated successfully." });
        }

        [HttpDelete("deleteassetCheckout{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id);
            return Ok(new { message = "AssetCheckout deleted successfully." });
        }
    }
}
