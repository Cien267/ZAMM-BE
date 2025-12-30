using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Loan;
using Zamm.Application.Payloads.ResultModels.Loan;
using Zamm.Shared.Models;

namespace Zamm.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/loan")]
public class LoanController : ControllerBase
{
    private readonly ILoanService _loanService;
    public LoanController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<LoanResult>>> GetListLoanAsync([FromQuery] LoanQuery query)
    {
        var result = await _loanService.GetListLoanAsync(query);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<LoanResult>> GetLoanByIdAsync(Guid id)
    {
        var result = await _loanService.GetLoanByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LoanResult>> CreateLoanAsync([FromBody] CreateLoanInput request)
    {
        var result = await _loanService.CreateLoanAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LoanResult>> UpdateLoanAsync(Guid id, [FromBody] UpdateLoanInput request)
    {
        var result = await _loanService.UpdateLoanAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoanAsync(Guid id)
    {
        await _loanService.DeleteLoanAsync(id);
        return NoContent();
    }
}
