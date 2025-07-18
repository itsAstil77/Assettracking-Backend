using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubSubCategoryController : ControllerBase
    {
        private readonly SubSubCategoryService _service;

        public SubSubCategoryController(SubSubCategoryService service)
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
                return NotFound("SubSub Category not found");
            return Ok(item);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SubSubCategoryRequest request)
        {
            try
            {
                await _service.CreateAsync(request);
                return Ok(new { message = "SubSub Category created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SubSubCategoryRequest request)
        {
            try
            {
                await _service.UpdateAsync(id, request);
                return Ok(new {message="SubSub Category updated successfully" });
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
        [HttpGet("GetSubsubcategorybysubcategory")]
        public async Task<IActionResult> GetSubsubcategorybysubcategory(string id)
        {
            var sub = await _service.GetSubsubcategorybysubcategory(id);
            return Ok(sub);
        }
    }
}
