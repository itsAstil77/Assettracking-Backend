using Microsoft.AspNetCore.Mvc;
using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;

namespace AssetTrackingAuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly AssetService _assetService;

        public AssetController(AssetService assetService)
        {
            _assetService = assetService;
        }

        // Add Asset(s)
        [HttpPost("add")]
        public async Task<IActionResult> AddAsset([FromBody] Asset asset)
        {
            var (success, message) = await _assetService.AddAssetAsync(asset);
            if (!success)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }

        // Summary: Get All Assets
        [HttpGet("summary")]
        public async Task<IActionResult> GetAllAssets()
        {
            var assets = await _assetService.GetAllAssetsAsync();
            return Ok(assets);
        }

        // Update Asset by Id
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAsset(string id, [FromBody] Asset updatedAsset)
        {
            var (success, message) = await _assetService.UpdateAssetAsync(id, updatedAsset);
            if (!success)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }

         [HttpGet("previewcodes")]
public async Task<IActionResult> PreviewCodes([FromQuery] int quantity)
{
    if (quantity <= 0)
        return BadRequest(new { error = "Quantity must be greater than 0" });

    var codes = await _assetService.PreviewNextAssetCodes(quantity);
    return Ok(codes);
}


        // Delete Asset by Id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsset(string id)
        {
            var (success, message) = await _assetService.DeleteAssetAsync(id);
            if (!success)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }
    }
}
