using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Lender;
using Zamm.Application.Payloads.ResultModels.Lender;
using Zamm.Shared.Models;

namespace Zamm.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/lender")]
public class LenderController : ControllerBase
{
    private readonly ILenderService _lenderService;
    public LenderController(ILenderService lenderService)
    {
        _lenderService = lenderService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<LenderResult>>> GetListLenderAsync([FromQuery] LenderQuery query)
    {
        var result = await _lenderService.GetListLenderAsync(query);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<LenderResult>> GetLenderByIdAsync(Guid id)
    {
        var result = await _lenderService.GetLenderByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LenderResult>> CreateLenderAsync([FromBody] CreateLenderInput request)
    {
        var result = await _lenderService.CreateLenderAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LenderResult>> UpdateLenderAsync(Guid id, [FromBody] UpdateLenderInput request)
    {
        var result = await _lenderService.UpdateLenderAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLenderAsync(Guid id)
    {
        await _lenderService.DeleteLenderAsync(id);
        return NoContent();
    }
}
