using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleAccessController : ControllerBase
    {
        private readonly RoleAccessService _roleAccessService;

        public RoleAccessController(RoleAccessService service)
        {
            _roleAccessService = service;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var list = await _roleAccessService.GetAllAsync(pageNumber, pageSize);
            var total = await _roleAccessService.GetTotalCountAsync();

            return Ok(new
            {
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Data = list
            });
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<RoleAccess>> Get(string id)
        {
            var result = await _roleAccessService.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(RoleAccess newRole)
        {
            try
            {
                await _roleAccessService.CreateAsync(newRole);
                return CreatedAtAction(nameof(Get), new { id = newRole.Id }, newRole);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id:length(24)}")]
public async Task<IActionResult> Update(string id, RoleAccess updated)
{
    var existing = await _roleAccessService.GetByIdAsync(id);
    if (existing is null) return NotFound();

    await _roleAccessService.UpdateAsync(id, updated);
    return Ok(new { message = "Updated successfully" });  // <-- return message
}


        [HttpDelete("delete/{id:length(24)}")]
public async Task<IActionResult> Delete(string id)
{
    var existing = await _roleAccessService.GetByIdAsync(id);
    if (existing is null) return NotFound();

    await _roleAccessService.DeleteAsync(id);
    return Ok(new { message = "Deleted successfully" });  
}
    }
}
