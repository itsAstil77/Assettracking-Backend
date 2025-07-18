using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HierarchyController : ControllerBase
    {
        private readonly HierarchyService _hierarchyService;

        public HierarchyController(HierarchyService hierarchyService)
        {
            _hierarchyService = hierarchyService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetHierarchy()
        {
            var data = await _hierarchyService.GetHierarchyAsync();
            return Ok(data);
        }
    }
}
