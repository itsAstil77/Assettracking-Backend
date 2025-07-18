using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainCategoryController : ControllerBase
    {
        private readonly MainCategoryService _service;

        public MainCategoryController(MainCategoryService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound("Main category not found");
            return Ok(category);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MainCategoryRequest request)
        {
            try
            {
                await _service.CreateAsync(request);
                return Ok(new { message = "Main category created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MainCategoryRequest request)
        {
            try
            {
                await _service.UpdateAsync(id, request);
                return Ok(new { message = "Main category updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var category = await _service.DeleteAsync(id);
            return Ok(new { message=category });
        }
        [HttpGet("GetMainCategorybygroup")]
        public async Task<IActionResult> GetMainCatogorybygroup(string id)
        {
           var main= await _service.GetMainCategorybygroup(id);
            return Ok(main);
        }
    }
}
