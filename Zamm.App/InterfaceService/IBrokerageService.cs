using Zamm.Application.Payloads.InputModels.Brokerage;
using Zamm.Application.Payloads.InputModels.Invitation;
using Zamm.Application.Payloads.ResultModels.Brokerage;
using Zamm.Application.Payloads.ResultModels.Invitation;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService;

public interface IBrokerageService
{
    Task<PagedResult<BrokerageResult>> GetListBrokerageAsync(BrokerageQuery query);
    Task<BrokerageResult> GetBrokerageByIdAsync(Guid id);
    Task<BrokerageResult> CreateBrokerageAsync(CreateBrokerageInput request);
    Task<BrokerageResult> UpdateBrokerageAsync(Guid brokerageId, UpdateBrokerageInput request);
    Task DeleteBrokerageAsync(Guid id);
    
    Task<InvitationResult> CreateInvitationAsync(CreateInvitationInput request);
    Task<List<InvitationResult>> GetInvitationsByBrokerageIdAsync(Guid brokerageId);
    Task DeleteInvitationAsync(Guid id);
}