using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly GroupService _groupService;

        public GroupController(GroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _groupService.GetAllAsync();
            return Ok(groups);
        }

        [HttpGet("get/{id:length(24)}")]
        public async Task<IActionResult> GetById(string id)
        {
            var group = await _groupService.GetByIdAsync(id);
            if (group == null)
                return NotFound(new { message = "Group not found" });

            return Ok(group);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] GroupRequest request)
        {
            await _groupService.CreateAsync(request);
            return Ok(new { message = "Group created successfully" });
        }

        [HttpPut("update/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] GroupRequest request)
        {
            try
            {
                await _groupService.UpdateAsync(id, request);
                return Ok(new { message = "Group updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

       [HttpDelete("delete/{id:length(24)}")]
public async Task<IActionResult> Delete(string id)
{
    var resultMessage = await _groupService.DeleteAsync(id);
    return Ok(new { message = resultMessage });
}

    }
}
