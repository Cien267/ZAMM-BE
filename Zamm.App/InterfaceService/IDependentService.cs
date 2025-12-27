using Zamm.Application.Payloads.InputModels.Dependent;
using Zamm.Application.Payloads.ResultModels.Depentdent;

namespace Zamm.Application.InterfaceService;

public interface IDependentService
{
    Task<List<DependentResult>> GetDependentsByPersonIdAsync(Guid personId);
    Task<DependentResult> GetDependentByIdAsync(Guid id);
    Task<DependentResult> CreateDependentAsync(CreateDependentInput request);
    Task<DependentResult> UpdateDependentAsync(UpdateDependentInput request);
    Task DeleteDependentAsync(Guid id);
}