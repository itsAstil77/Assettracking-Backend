using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }
[HttpGet("Getroombyfloor")]
        public async Task<ActionResult<List<Room>>> Getfloorbybuilding( string FloorId)
        {
            var canteens = await _roomService.GetAllRoombyfloorAsync(FloorId);
            return Ok(canteens);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return NotFound("Room not found.");
            return Ok(room);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoomRequest request)
        {
            try
            {
                await _roomService.CreateAsync(request);
                return Ok(new { message = "Room created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RoomRequest request)
        {
            try
            {
                await _roomService.UpdateAsync(id, request);
                return Ok(new { message = "Room updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _roomService.DeleteAsync(id);
                return Ok(new { message = "Room deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
