using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Asset;
using Zamm.Application.Payloads.ResultModels.Asset;
using Zamm.Shared.Models;

namespace Zamm.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/asset")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        public AssetController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AssetResult>>> GetListAssetAsync([FromQuery] AssetQuery query)
        {
            var result = await _assetService.GetListAssetAsync(query);
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetResult>> GetAssetByIdAsync(Guid id)
        {
            var result = await _assetService.GetAssetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AssetResult>> CreateAssetAsync([FromBody] CreateAssetInput request)
        {
            var result = await _assetService.CreateAssetAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AssetResult>> UpdateAssetAsync(Guid id, [FromBody] UpdateAssetInput request)
        {
            var result = await _assetService.UpdateAssetAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetAsync(Guid id)
        {
            await _assetService.DeleteAssetAsync(id);
            return NoContent();
        }
    }
}
