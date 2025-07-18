using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoryController : ControllerBase
    {
        private readonly SubCategoryService _service;

        public SubCategoryController(SubCategoryService service)
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
            var sub = await _service.GetByIdAsync(id);
            if (sub == null)
                return NotFound("Sub Category not found");
            return Ok(sub);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SubCategoryRequest request)
        {
            try
            {
                await _service.CreateAsync(request);
                return Ok(new { message="Sub Category created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Getsubcategorybymaincategory")]
        public async Task<IActionResult> Getsubcategorybymaincategory(string id)
        {
            var sub = await _service.Getsubcategorybymaincategory(id);
            return Ok(sub);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SubCategoryRequest request)
        {
            try
            {
                await _service.UpdateAsync(id, request);
                return Ok(new { Message = "Sub Category updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var del = await _service.DeleteAsync(id);
            return Ok(new { message = del });
        }
    }
}
