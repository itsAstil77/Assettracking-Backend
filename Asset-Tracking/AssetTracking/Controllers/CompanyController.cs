using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll() => Ok(await _companyService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();
            return Ok(company);
        }
        [HttpGet("company-summary")]
        public async Task<ActionResult<List<Company>>> GetCompanyByGroup(string GroupId)
        {
            var canteens = await _companyService.GetCompanyByGroupidIdAsync(GroupId);
            return Ok(canteens);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompanyRequest request)
        {
            try
            {
                await _companyService.CreateAsync(request);
                return Ok(new { message = "Company created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, CompanyRequest request)
        {
            try
            {
                await _companyService.UpdateAsync(id, request);
                return Ok(new { message = "Company updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var del = await _companyService.DeleteAsync(id);
            return Ok(new { message = del });
        }
    }
}
