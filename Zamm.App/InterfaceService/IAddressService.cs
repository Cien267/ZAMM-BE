using Zamm.Application.Payloads.InputModels.Address;

namespace Zamm.Application.InterfaceService;

public interface IAddressService
{
    Task<Guid?> UpsertAddressAsync (CreateAddressInput? addressInput, Guid? currentAddressId = null);
}