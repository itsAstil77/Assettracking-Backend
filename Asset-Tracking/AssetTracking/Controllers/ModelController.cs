using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly ModelService _service;

        public ModelController(ModelService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { message = "Model not found" });
            return Ok(item);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ModelRequest request)
        {
            try
            {
                await _service.CreateAsync(request);
                return Ok(new { message = "Model created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ModelRequest request)
        {
            try
            {
                await _service.UpdateAsync(id, request);
                return Ok(new { message = "Model updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { message = "Model not found" });

            await _service.DeleteAsync(id);
            return Ok(new { message = "Model deleted successfully" });
        }

        [HttpGet("getmodelbybrand")]
        public async Task<IActionResult> Getmodelbybrand(string id)
        {
            var model = await _service.getmodelbybrand(id);
            return Ok(model);
        }
    }
}
