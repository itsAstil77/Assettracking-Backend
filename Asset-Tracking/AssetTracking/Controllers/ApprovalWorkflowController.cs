using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using AssetTrackingAuthAPI.Config;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalWorkflowController : ControllerBase
    {
        private readonly ApprovalWorkflowService _approval;
        public ApprovalWorkflowController(ApprovalWorkflowService app)
        {
            _approval = app;
        }

        [HttpGet("workflowsummary")]
        public async Task<IActionResult> getallworkflow()
        {
            var list = await _approval.GetAllWorkflowAsync();
            return Ok(list);
        }

        [HttpPost("addWorkflow")]
        public async Task<IActionResult> createWorkflow([FromBody] ApprovalWorkflow app)
        {
            var get = await _approval.Createworkflow(app);
            return Ok(new { message = "Workflow Added Sucessfully" });
        }
        [HttpPut("updateworkflow")]
        public async Task<IActionResult> UpdateWorkflow([FromBody] ApprovalWorkflow app, string id)
        {
            var (sucess, message) = await _approval.updateworkflowAsync(id, app);
            if (!sucess)
                return BadRequest(new { error = message });
            else
                return Ok(new { Sucess = message });
        }
        [HttpDelete("deleteworkflow")]
        public async Task<IActionResult> deleteworkflow(string id)
        {
            var (sucess, message) = await _approval.deleteworkflow(id);
            if (!sucess)
                return BadRequest(new { message });
            else
                return Ok(new { message });    
       }
}

}

