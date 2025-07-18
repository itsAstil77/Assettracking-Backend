using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorController : ControllerBase
    {
        private readonly FloorService _floorService;

        public FloorController(FloorService floorService)
        {
            _floorService = floorService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var floors = await _floorService.GetAllAsync();
            return Ok(floors);
        }
[HttpGet("Getfloorbybuilding")]
        public async Task<ActionResult<List<Floor>>> Getfloorbybuilding( string BuildingId)
        {
            var canteens = await _floorService.GetAllfloorbybuildingAsync(BuildingId);
            return Ok(canteens);
        }
        [HttpGet("get {id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var floor = await _floorService.GetByIdAsync(id);
            if (floor == null)
                return NotFound(new { message = "Floor not found." });
            return Ok(floor);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FloorRequest request)
        {
            try
            {
                await _floorService.CreateAsync(request);
                return Ok(new { message = "Floor created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] FloorRequest request)
        {
            try
            {
                await _floorService.UpdateAsync(id, request);
                return Ok(new { message = "Floor updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var del = await _floorService.DeleteAsync(id);
            return Ok(new { message = del });
        }
    }
}
