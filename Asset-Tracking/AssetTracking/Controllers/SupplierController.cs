using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierService _supplierService;

        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SupplierRequest request)
        {
            try
            {
                await _supplierService.CreateAsync(request);
                return Ok(new { message = "Supplier created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SupplierRequest request)
        {
            try
            {
                await _supplierService.UpdateAsync(id, request);
                return Ok(new { message = "Supplier updated successfully." });
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
                await _supplierService.DeleteAsync(id);
                return Ok(new { message = "Supplier deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
