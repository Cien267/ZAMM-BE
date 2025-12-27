using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Dependent;
using Zamm.Application.Payloads.ResultModels.Depentdent;

namespace Zamm.Controllers;

[ApiController]
[Route("api/dependent")]
public class DependentController : ControllerBase
{
    private readonly IDependentService _dependentService;

    public DependentController(IDependentService dependentService)
    {
        _dependentService = dependentService;
    }

    [HttpGet("person/{personId}")]
    public async Task<ActionResult<List<DependentResult>>> GetDependentsByPersonIdAsync(Guid personId)
    {
        var result = await _dependentService.GetDependentsByPersonIdAsync(personId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DependentResult>> GetDependentByIdAsync(Guid id)
    {
        var result = await _dependentService.GetDependentByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DependentResult>> CreateDependentAsync([FromBody] CreateDependentInput request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _dependentService.CreateDependentAsync(request);
        return CreatedAtAction(nameof(GetDependentByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DependentResult>> UpdateDependentAsync(Guid id, [FromBody] UpdateDependentInput request)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "ID in URL does not match ID in request body" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _dependentService.UpdateDependentAsync(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDependentAsync(Guid id)
    {
        await _dependentService.DeleteDependentAsync(id);
        return NoContent();
    }
}