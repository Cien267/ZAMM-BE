using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Address;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;

namespace Zamm.Application.ImplementService;

public class AddressService : IAddressService
{
    private readonly IBaseRepository<Address> _baseAddressRepository;
    public AddressService(
        IBaseRepository<Address> baseAddressRepository)
    {
        _baseAddressRepository = baseAddressRepository;
    }
    
    public async Task<Guid?> UpsertAddressAsync(CreateAddressInput? addressInput, Guid? currentAddressId = null)
    {
        if (addressInput == null)
        {
            return null;
        }

        if (addressInput.Id.HasValue)
        {
            var existingAddress = await _baseAddressRepository.GetByIdAsync(addressInput.Id.Value);
            if (existingAddress != null)
            {
                existingAddress.Level = addressInput.Level;
                existingAddress.Building = addressInput.Building;
                existingAddress.UnitNumber = addressInput.UnitNumber;
                existingAddress.StreetNumber = addressInput.StreetNumber;
                existingAddress.StreetName = addressInput.StreetName;
                existingAddress.Suburb = addressInput.Suburb;
                existingAddress.State = addressInput.State;
                existingAddress.Country = addressInput.Country;
                existingAddress.Postcode = addressInput.Postcode;
                existingAddress.OffPlan = addressInput.OffPlan;

                await _baseAddressRepository.UpdateAsync(existingAddress);
                return existingAddress.Id;
            }
        }

        var newAddress = new Address
        {
            Level = addressInput.Level,
            Building = addressInput.Building,
            UnitNumber = addressInput.UnitNumber,
            StreetNumber = addressInput.StreetNumber,
            StreetName = addressInput.StreetName,
            Suburb = addressInput.Suburb,
            State = addressInput.State,
            Country = addressInput.Country,
            Postcode = addressInput.Postcode,
            OffPlan = addressInput.OffPlan,
        };

        await _baseAddressRepository.CreateAsync(newAddress);
        return newAddress.Id;
    }
}