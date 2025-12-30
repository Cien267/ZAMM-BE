using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Company;
using Zamm.Application.Payloads.ResultModels.Company;
using Zamm.Shared.Models;

namespace Zamm.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/company")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CompanyResult>>> GetListCompanyAsync([FromQuery] CompanyQuery query)
    {
        var result = await _companyService.GetListCompanyAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyResult>> GetCompanyByIdAsync(Guid id)
    {
        var result = await _companyService.GetCompanyByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyResult>> CreateCompanyAsync([FromBody] CreateCompanyInput request)
    {
        var result = await _companyService.CreateCompanyAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CompanyResult>> UpdateCompanyAsync(Guid id, [FromBody] UpdateCompanyInput request)
    {
        var result = await _companyService.UpdateCompanyAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompanyAsync(Guid id)
    {
        await _companyService.DeleteCompanyAsync(id);
        return NoContent();
    }
}