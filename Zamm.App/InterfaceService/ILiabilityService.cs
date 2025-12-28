using Zamm.Application.Payloads.InputModels.Liability;
using Zamm.Application.Payloads.ResultModels.Liability;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService;

public interface ILiabilityService
{
    Task<PagedResult<LiabilityResult>> GetListLiabilityAsync(LiabilityQuery query);
    Task<LiabilityResult> GetLiabilityByIdAsync(Guid id);
    Task<LiabilityResult> CreateLiabilityAsync(CreateLiabilityInput request);
    Task<LiabilityResult> UpdateLiabilityAsync(Guid liabilityId, UpdateLiabilityInput request);
    Task DeleteLiabilityAsync(Guid id);
}