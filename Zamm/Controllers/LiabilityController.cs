using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Liability;
using Zamm.Application.Payloads.ResultModels.Liability;
using Zamm.Shared.Models;

namespace Zamm.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/liability")]
    public class LiabilityController : ControllerBase
    {
        private readonly ILiabilityService _liabilityService;
        public LiabilityController(ILiabilityService liabilityService)
        {
            _liabilityService = liabilityService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<LiabilityResult>>> GetListLiabilityAsync([FromQuery] LiabilityQuery query)
        {
            var result = await _liabilityService.GetListLiabilityAsync(query);
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<LiabilityResult>> GetLiabilityByIdAsync(Guid id)
        {
            var result = await _liabilityService.GetLiabilityByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<LiabilityResult>> CreateLiabilityAsync([FromBody] CreateLiabilityInput request)
        {
            var result = await _liabilityService.CreateLiabilityAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LiabilityResult>> UpdateLiabilityAsync(Guid id, [FromBody] UpdateLiabilityInput request)
        {
            var result = await _liabilityService.UpdateLiabilityAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLiabilityAsync(Guid id)
        {
            await _liabilityService.DeleteLiabilityAsync(id);
            return NoContent();
        }
    }
}
