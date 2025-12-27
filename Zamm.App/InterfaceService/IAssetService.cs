using Zamm.Application.Payloads.InputModels.Asset;
using Zamm.Application.Payloads.ResultModels.Asset;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService
{
    public interface IAssetService
    {
        Task<PagedResult<AssetResult>> GetListAssetAsync(AssetQuery query);
        Task<AssetResult> GetAssetByIdAsync(Guid id);
        Task<AssetResult> CreateAssetAsync(CreateAssetInput request);
        Task<AssetResult> UpdateAssetAsync(Guid assetId, UpdateAssetInput request);
        Task DeleteAssetAsync(Guid id);
    }
}
