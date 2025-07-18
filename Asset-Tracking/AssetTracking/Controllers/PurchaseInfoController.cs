using Microsoft.AspNetCore.Mvc;
using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseInfoController : ControllerBase
    {
        private readonly PurchaseInfoService _service;

        public PurchaseInfoController(PurchaseInfoService service)
        {
            _service = service;
        }

        // [HttpPost("create/{assetId}")]
        // public async Task<IActionResult> Create(string assetId, [FromBody] PurchaseInfo info)
        // {
        //     var (success, message) = await _service.AddPurchaseInfoAsync(assetId, info);
        //     return success ? Ok(new { message }) : NotFound(new { error = message });
        // }
        [HttpPost("createPurchaseInfor")]
        public async Task<IActionResult> create([FromBody] PurchaseInfo purchase)
        {
            var(sucess,message)=await _service.CreateAsycn(purchase);
            if (!sucess)
                return BadRequest(new { error = message });

            return Ok(new { message  });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllPurchaseInfoAsync();
            return Ok(list);
        }
        [HttpPut("UpdatePurchaseInfo/{id}")]
        public async Task<IActionResult> UpdatePurchaseInfo(string id, [FromBody] PurchaseInfo purchase)
        {
            var (sucess, Message) = await _service.UpdatePurchaseInfoAsync(purchase, id);
            if (!sucess)
                return BadRequest(new { error = Message });
            else
                return Ok(new { Message });

        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
           var (Message,sucess)= await _service.DeletePurchaseInfo(id);
            if (!sucess)
                return BadRequest(new { Message="no purchase info found"});
            else    
                 return Ok(new { message = "deleted Sucessfully" });
        }

        // [HttpPut("update/{assetId}")]
        // public async Task<IActionResult> Update(string assetId, [FromBody] PurchaseInfo info)
        // {
        //     var (success, message) = await _service.UpdatePurchaseInfoAsync(assetId, info);
        //     return success ? Ok(new { message }) : NotFound(new { error = message });
        // }

        // [HttpDelete("delete/{assetId}")]
        // public async Task<IActionResult> Delete(string assetId)
        // {
        //     var (success, message) = await _service.DeletePurchaseInfoAsync(assetId);
        //     return success ? Ok(new { message }) : NotFound(new { error = message });
        // }
    }
}