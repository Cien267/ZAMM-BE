using Zamm.Application.Payloads.InputModels.Lender;
using Zamm.Application.Payloads.ResultModels.Lender;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService;

public interface ILenderService
{
    Task<PagedResult<LenderResult>> GetListLenderAsync(LenderQuery query);
    Task<LenderResult> GetLenderByIdAsync(Guid id);
    Task<LenderResult> CreateLenderAsync(CreateLenderInput request);
    Task<LenderResult> UpdateLenderAsync(Guid lenderId, UpdateLenderInput request);
    Task DeleteLenderAsync(Guid id);
}