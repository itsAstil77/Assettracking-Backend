using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectorController : ControllerBase
    {
        private readonly SectorService _sectorService;

        public SectorController(SectorService sectorService)
        {
            _sectorService = sectorService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var sectors = await _sectorService.GetAllAsync();
            return Ok(sectors);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SectorRequest request)
        {
            try
            {
                await _sectorService.CreateAsync(request);
                return Ok(new { message = "Sector created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SectorRequest request)
        {
            try
            {
                await _sectorService.UpdateAsync(id, request);
                return Ok(new { message = "Sector updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _sectorService.DeleteAsync(id);
                return Ok(new { message = "Sector deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
