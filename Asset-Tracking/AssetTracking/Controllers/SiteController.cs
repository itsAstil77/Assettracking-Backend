using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/sites")]
    public class SiteController : ControllerBase
    {
        private readonly SiteService _siteService;

        public SiteController(SiteService siteService)
        {
            _siteService = siteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _siteService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var site = await _siteService.GetByIdAsync(id);
            if (site == null) return NotFound();
            return Ok(site);
        }
         [HttpGet("Getsitesbycompany")]
        public async Task<ActionResult<List<Site>>> GetsiteByCompany( string CompanyId)
        {
            var canteens = await _siteService.GetAllSitesbycompanyidAsync(CompanyId);
            return Ok(canteens);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SiteRequest request)
        {
            try
            {
                await _siteService.CreateAsync(request);
                return Ok(new { message = "Site created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, SiteRequest request)
        {
            try
            {
                await _siteService.UpdateAsync(id, request);
                return Ok(new { message = "Site updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var del = await _siteService.DeleteAsync(id);
            return Ok(new { message = del });
        }
    }
}
