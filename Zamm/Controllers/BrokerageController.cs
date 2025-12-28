using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Brokerage;
using Zamm.Application.Payloads.InputModels.Invitation;
using Zamm.Application.Payloads.ResultModels.Brokerage;
using Zamm.Application.Payloads.ResultModels.Invitation;
using Zamm.Shared.Models;

namespace Zamm.Controllers;

[ApiController]
[Route("api/brokerage")]
public class BrokerageController : ControllerBase
{
    private readonly IBrokerageService _brokerageService;

    public BrokerageController(IBrokerageService brokerageService)
    {
        _brokerageService = brokerageService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BrokerageResult>>> GetListBrokerageAsync([FromQuery] BrokerageQuery query)
    {
        var result = await _brokerageService.GetListBrokerageAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrokerageResult>> GetBrokerageByIdAsync(Guid id)
    {
        var result = await _brokerageService.GetBrokerageByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BrokerageResult>> CreateBrokerageAsync([FromBody] CreateBrokerageInput request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _brokerageService.CreateBrokerageAsync(request);
        return CreatedAtAction(nameof(GetBrokerageByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BrokerageResult>> UpdateBrokerageAsync(Guid id, [FromBody] UpdateBrokerageInput request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _brokerageService.UpdateBrokerageAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBrokerageAsync(Guid id)
    {
        await _brokerageService.DeleteBrokerageAsync(id);
        return NoContent();
    }

    [HttpPost("invitations")]
    public async Task<ActionResult<InvitationResult>> CreateInvitationAsync([FromBody] CreateInvitationInput request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _brokerageService.CreateInvitationAsync(request);
        return Ok(result);
    }

    [HttpGet("{brokerageId}/invitations")]
    public async Task<ActionResult<List<InvitationResult>>> GetInvitationsByBrokerageIdAsync(Guid brokerageId)
    {
        var result = await _brokerageService.GetInvitationsByBrokerageIdAsync(brokerageId);
        return Ok(result);
    }

    [HttpDelete("invitations/{id}")]
    public async Task<IActionResult> DeleteInvitationAsync(Guid id)
    {
        await _brokerageService.DeleteInvitationAsync(id);
        return NoContent();
    }
}