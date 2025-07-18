using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HierarchyCategoryController : ControllerBase
    {
        private readonly HierarchyCategoryService _service;

        public HierarchyCategoryController(HierarchyCategoryService service)
        {
            _service = service;
        }

        [HttpGet("category-summary")]
        public async Task<IActionResult> GetCategoryHierarchy()
        {
            var result = await _service.GetFullCategoryHierarchyAsync();
            return Ok(result);
        }
    }
}
