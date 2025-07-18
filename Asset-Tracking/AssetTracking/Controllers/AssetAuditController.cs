using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using YourNamespace.Services;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetAuditController : ControllerBase
    {
        private readonly AssetAuditService _auditService;

        public AssetAuditController(AssetAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("assetauditsummary")]
        public async Task<ActionResult<List<AssetAudit>>> GetAll() =>
            Ok(await _auditService.GetAllAsync());

        [HttpGet("getauditasset/{id}")]
        public async Task<ActionResult<AssetAudit>> GetById(string id)
        {
            var audit = await _auditService.GetByIdAsync(id);
            return audit is null ? NotFound() : Ok(audit);
        }

        [HttpPost ("addassetaudit")]
        public async Task<IActionResult> Create([FromBody] AssetAudit audit)
        {
            audit.CreatedDate = DateTime.UtcNow;
            await _auditService.AddAsync(audit);
            return CreatedAtAction(nameof(GetById), new { id = audit.Id }, audit);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AssetAudit audit)
        {
            audit.Id = id;
            var updated = await _auditService.UpdateAsync(id, audit);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _auditService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
