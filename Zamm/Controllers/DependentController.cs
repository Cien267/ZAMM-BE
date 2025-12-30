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
        var result = await _dependentService.CreateDependentAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DependentResult>> UpdateDependentAsync(Guid id, [FromBody] UpdateDependentInput request)
    {
        var result = await _dependentService.UpdateDependentAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDependentAsync(Guid id)
    {
        await _dependentService.DeleteDependentAsync(id);
        return NoContent();
    }
}