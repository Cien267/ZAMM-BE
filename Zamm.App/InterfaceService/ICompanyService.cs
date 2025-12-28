using Zamm.Application.Payloads.InputModels.Company;
using Zamm.Application.Payloads.ResultModels.Company;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService;

public interface ICompanyService
{
    Task<PagedResult<CompanyResult>> GetListCompanyAsync(CompanyQuery query);
    Task<CompanyResult> GetCompanyByIdAsync(Guid id);
    Task<CompanyResult> CreateCompanyAsync(CreateCompanyInput request);
    Task<CompanyResult> UpdateCompanyAsync(Guid companyId, UpdateCompanyInput request);
    Task DeleteCompanyAsync(Guid id);
}