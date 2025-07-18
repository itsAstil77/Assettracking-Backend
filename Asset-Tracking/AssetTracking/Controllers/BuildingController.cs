using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingController : ControllerBase
    {
        private readonly BuildingService _buildingService;

        public BuildingController(BuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var buildings = await _buildingService.GetAllAsync();
            return Ok(buildings);
        }
 [HttpGet("Getbuildingsbysites")]
        public async Task<ActionResult<List<Building>>> GetsiteByCompany( string SiteId)
        {
            var canteens = await _buildingService.GetAllbuildingsbysiteAsync(SiteId);
            return Ok(canteens);
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var building = await _buildingService.GetByIdAsync(id);
            if (building == null) return NotFound("Building not found.");
            return Ok(building);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BuildingRequest request)
        {
            try
            {
                await _buildingService.CreateAsync(request);
                return Ok(new { message="Building created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BuildingRequest request)
        {
            try
            {
                await _buildingService.UpdateAsync(id, request);
                return Ok(new { message = "Building updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new{message=ex.Message});
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _buildingService.DeleteAsync(id);
                return Ok(new { message = "Building deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
