using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly BrandService _service;

        public BrandController(BrandService service)
        {
            _service = service;
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { message = "Brand not found" });
            return Ok(item);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BrandRequest request)
        {
            try
            {
                await _service.CreateAsync(request);
                return Ok(new { message = "Brand created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BrandRequest request)
        {
            try
            {
                await _service.UpdateAsync(id, request);
                return Ok(new { message = "Brand updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("getbrandbysubsubcategory")]
        public async Task<IActionResult> GetBrandbysubsubcategory(string id)
        {
            var brand = await _service.getbrandbysubsubcategory(id);
            return Ok(brand);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var del = await _service.DeleteAsync(id);
            return Ok(new { message = del });
        }
    }
}
