using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustodianController : ControllerBase
    {
        private readonly CustodianService _custodianService;

        public CustodianController(CustodianService custodianService)
        {
            _custodianService = custodianService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var custodians = await _custodianService.GetAllAsync();
            return Ok(custodians);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CustodianRequest request)
        {
            try
            {
                await _custodianService.CreateAsync(request);
                return Ok(new { message = "Custodian created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustodianRequest request)
        {
            try
            {
                await _custodianService.UpdateAsync(id, request);
                return Ok(new { message = "Custodian updated successfully." });
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
                await _custodianService.DeleteAsync(id);
                return Ok(new { message = "Custodian deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
