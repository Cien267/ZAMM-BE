using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Address;
using Zamm.Application.Payloads.InputModels.Asset;
using Zamm.Application.Payloads.Responses;
using Zamm.Shared.Models;
using Zamm.Application.Payloads.ResultModels.Asset;

namespace Zamm.Application.ImplementService
{
    public class AssetService : IAssetService
    {
        private readonly IBaseRepository<Asset> _baseAssetRepository;
        private readonly IBaseRepository<Address> _baseAddressRepository;
        private readonly IBaseRepository<Person> _basePersonRepository;
        private readonly IBaseRepository<Company> _baseCompanyRepository;
        private readonly IBaseRepository<AssetPerson> _baseAssetPersonRepository;
        private readonly IBaseRepository<AssetCompany> _baseAssetCompanyRepository;
        private readonly IBaseRepository<LiabilityAsset> _baseLiabilityAssetRepository;
        private readonly IBaseRepository<Liability> _baseLiabilityRepository;
        private readonly IAddressService _addressService;
        public AssetService(
            IBaseRepository<Asset> baseAssetRepository,
            IBaseRepository<Address> baseAddressRepository,
            IBaseRepository<Person> basePersonRepository,
            IBaseRepository<Company> baseCompanyRepository,
            IBaseRepository<AssetPerson> baseAssetPersonRepository,
            IBaseRepository<AssetCompany> baseAssetCompanyRepository,
            IBaseRepository<LiabilityAsset> baseLiabilityAssetRepository,
            IBaseRepository<Liability> baseLiabilityRepository,
            IAddressService addressService)
        {
            _baseAssetRepository = baseAssetRepository;
            _baseAddressRepository = baseAddressRepository;
            _basePersonRepository = basePersonRepository;
            _baseCompanyRepository = baseCompanyRepository;
            _baseAssetPersonRepository = baseAssetPersonRepository;
            _baseAssetCompanyRepository = baseAssetCompanyRepository;
            _baseLiabilityAssetRepository = baseLiabilityAssetRepository;
            _baseLiabilityRepository = baseLiabilityRepository;
            _addressService = addressService;
        }

        public async Task<PagedResult<AssetResult>> GetListAssetAsync(AssetQuery assetQuery)
        {
            var query = _baseAssetRepository.BuildQueryable(
                new List<string> { "Address", "AssetPeople.Person", "AssetCompanies.Company", "LiabilityAssets.Liability" },
                null
            );

            if (!string.IsNullOrEmpty(assetQuery.Name))
            {
                var name = assetQuery.Name.ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(name));
            }

            if (assetQuery.IsInvestment.HasValue)
            {
                query = query.Where(a => a.IsInvestment == assetQuery.IsInvestment.Value);
            }

            if (!string.IsNullOrEmpty(assetQuery.PropertyType))
            {
                var propertyType = assetQuery.PropertyType.ToLower();
                query = query.Where(a => a.PropertyType != null && a.PropertyType.ToLower().Contains(propertyType));
            }

            if (!string.IsNullOrEmpty(assetQuery.ZoningType))
            {
                var zoningType = assetQuery.ZoningType.ToLower();
                query = query.Where(a => a.ZoningType != null && a.ZoningType.ToLower().Contains(zoningType));
            }

            var totalCount = await query.CountAsync();

            query = _baseAssetRepository.ApplySorting(query, assetQuery.SortBy, assetQuery.SortDescending);

            var results = await query
                .Skip((assetQuery.PageNumber - 1) * assetQuery.PageSize)
                .Take(assetQuery.PageSize)
                .Select(AssetResult.FromAsset)
                .ToListAsync();

            return new PagedResult<AssetResult>(results, totalCount, assetQuery.PageNumber, assetQuery.PageSize);
        }
        
        public async Task<AssetResult> GetAssetByIdAsync(Guid id)
        {
            var asset = await _baseAssetRepository
                .BuildQueryable(
                    new List<string> { "Address", "AssetPeople.Person", "AssetCompanies.Company", "LiabilityAssets.Liability" },
                    a => a.Id == id
                )
                .Select(AssetResult.FromAsset)
                .FirstOrDefaultAsync();

            if (asset == null)
            {
                throw new ResponseErrorObject("Asset not found", StatusCodes.Status404NotFound);
            }

            return asset;
        }

        public async Task<AssetResult> CreateAssetAsync(CreateAssetInput request)
        {
            if (request.AssetPeople != null && request.AssetPeople.Any())
            {
                foreach (var ap in request.AssetPeople)
                {
                    var personExists = await _basePersonRepository.GetByIdAsync(ap.PersonId);
                    if (personExists == null)
                    {
                        throw new ResponseErrorObject($"Person with ID {ap.PersonId} not found", StatusCodes.Status400BadRequest);
                    }
                }

                var totalPersonPercent = request.AssetPeople.Sum(ap => ap.Percent);
                var totalCompanyPercent = request.AssetCompanies?.Sum(ac => ac.Percent) ?? 0;
                if (totalPersonPercent + totalCompanyPercent > 100)
                {
                    throw new ResponseErrorObject("Total ownership percentage cannot exceed 100%", StatusCodes.Status400BadRequest);
                }
            }

            if (request.AssetCompanies != null && request.AssetCompanies.Any())
            {
                foreach (var ac in request.AssetCompanies)
                {
                    var companyExists = await _baseCompanyRepository.GetByIdAsync(ac.CompanyId);
                    if (companyExists == null)
                    {
                        throw new ResponseErrorObject($"Company with ID {ac.CompanyId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }
            
            if (request.AssetLiabilities != null && request.AssetLiabilities.Any())
            {
                foreach (var la in request.AssetLiabilities)
                {
                    var liabilityExists = await _baseLiabilityRepository.GetByIdAsync(la.LiabilityId);
                    if (liabilityExists == null)
                    {
                        throw new ResponseErrorObject($"Liability with ID {la.LiabilityId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }

            var addressId = await _addressService.UpsertAddressAsync(request.Address);

            var asset = new Asset
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                AddressId = addressId,
                AddressOffPlan = request.AddressOffPlan,
                PropertyType = request.PropertyType,
                ZoningType = request.ZoningType,
                Value = request.Value,
                ValueIsCertified = request.ValueIsCertified,
                ValuationDate = request.ValuationDate,
                IsInvestment = request.IsInvestment,
                RentalIncomeValue = request.RentalIncomeValue,
                RentalIncomeFrequency = request.RentalIncomeFrequency,
                RentalHasAgent = request.RentalHasAgent,
                RentalAgentContact = request.RentalAgentContact,
                IsUnencumbered = request.IsUnencumbered
            };

            await _baseAssetRepository.CreateAsync(asset);

            if (request.AssetPeople != null && request.AssetPeople.Any())
            {
                var assetPeople = request.AssetPeople.Select(ap => new AssetPerson
                {
                    Id = Guid.NewGuid(),
                    AssetId = asset.Id,
                    PersonId = ap.PersonId,
                    Percent = ap.Percent
                }).ToList();

                await _baseAssetPersonRepository.CreateAsync(assetPeople);
            }

            if (request.AssetCompanies != null && request.AssetCompanies.Any())
            {
                var assetCompanies = request.AssetCompanies.Select(ac => new AssetCompany
                {
                    Id = Guid.NewGuid(),
                    AssetId = asset.Id,
                    CompanyId = ac.CompanyId,
                    Percent = ac.Percent
                }).ToList();

                await _baseAssetCompanyRepository.CreateAsync(assetCompanies);
            }
            
            if (request.AssetLiabilities != null && request.AssetLiabilities.Any())
            {
                var liabilityAssets = request.AssetLiabilities.Select(la => new LiabilityAsset
                {
                    Id = Guid.NewGuid(),
                    AssetId = asset.Id,
                    LiabilityId = la.LiabilityId
                }).ToList();

                await _baseLiabilityAssetRepository.CreateAsync(liabilityAssets);
            }

            return await GetAssetByIdAsync(asset.Id);
        }

        public async Task<AssetResult> UpdateAssetAsync(Guid assetId, UpdateAssetInput request)
        {
            var asset = await _baseAssetRepository.GetByIdAsync(assetId);
            if (asset == null)
            {
                throw new ResponseErrorObject("Asset not found", StatusCodes.Status404NotFound);
            }

            if (request.AssetPeople != null && request.AssetPeople.Any())
            {
                foreach (var ap in request.AssetPeople)
                {
                    var personExists = await _basePersonRepository.GetByIdAsync(ap.PersonId);
                    if (personExists == null)
                    {
                        throw new ResponseErrorObject($"Person with ID {ap.PersonId} not found", StatusCodes.Status400BadRequest);
                    }
                }

                var totalPersonPercent = request.AssetPeople.Sum(ap => ap.Percent);
                var totalCompanyPercent = request.AssetCompanies?.Sum(ac => ac.Percent) ?? 0;
                if (totalPersonPercent + totalCompanyPercent > 100)
                {
                    throw new ResponseErrorObject("Total ownership percentage cannot exceed 100%", StatusCodes.Status400BadRequest);
                }
            }

            if (request.AssetCompanies != null && request.AssetCompanies.Any())
            {
                foreach (var ac in request.AssetCompanies)
                {
                    var companyExists = await _baseCompanyRepository.GetByIdAsync(ac.CompanyId);
                    if (companyExists == null)
                    {
                        throw new ResponseErrorObject($"Company with ID {ac.CompanyId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }
            
            if (request.AssetLiabilities != null && request.AssetLiabilities.Any())
            {
                foreach (var la in request.AssetLiabilities)
                {
                    var liabilityExists = await _baseLiabilityRepository.GetByIdAsync(la.LiabilityId);
                    if (liabilityExists == null)
                    {
                        throw new ResponseErrorObject($"Liability with ID {la.LiabilityId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }

            var addressId = await _addressService.UpsertAddressAsync(request.Address, asset.AddressId);

            asset.Name = request.Name;
            asset.AddressId = addressId;
            asset.AddressOffPlan = request.AddressOffPlan;
            asset.PropertyType = request.PropertyType;
            asset.ZoningType = request.ZoningType;
            asset.Value = request.Value;
            asset.ValueIsCertified = request.ValueIsCertified;
            asset.ValuationDate = request.ValuationDate;
            asset.IsInvestment = request.IsInvestment;
            asset.RentalIncomeValue = request.RentalIncomeValue;
            asset.RentalIncomeFrequency = request.RentalIncomeFrequency;
            asset.RentalHasAgent = request.RentalHasAgent;
            asset.RentalAgentContact = request.RentalAgentContact;
            asset.IsUnencumbered = request.IsUnencumbered;

            await _baseAssetRepository.UpdateAsync(asset);

            if (request.AssetPeople != null)
            {
                await _baseAssetPersonRepository.DeleteAsync(ap => ap.AssetId == assetId);

                if (request.AssetPeople.Any())
                {
                    var assetPeople = request.AssetPeople.Select(ap => new AssetPerson
                    {
                        Id = Guid.NewGuid(),
                        AssetId = asset.Id,
                        PersonId = ap.PersonId,
                        Percent = ap.Percent
                    }).ToList();

                    await _baseAssetPersonRepository.CreateAsync(assetPeople);
                }
            }

            if (request.AssetCompanies != null)
            {
                await _baseAssetCompanyRepository.DeleteAsync(ac => ac.AssetId == assetId);

                if (request.AssetCompanies.Any())
                {
                    var assetCompanies = request.AssetCompanies.Select(ac => new AssetCompany
                    {
                        Id = Guid.NewGuid(),
                        AssetId = asset.Id,
                        CompanyId = ac.CompanyId,
                        Percent = ac.Percent
                    }).ToList();

                    await _baseAssetCompanyRepository.CreateAsync(assetCompanies);
                }
            }
            
            if (request.AssetLiabilities != null)
            {
                await _baseLiabilityAssetRepository.DeleteAsync(la => la.AssetId == assetId);

                if (request.AssetLiabilities.Any())
                {
                    var liabilityAssets = request.AssetLiabilities.Select(la => new LiabilityAsset
                    {
                        Id = Guid.NewGuid(),
                        AssetId = asset.Id,
                        LiabilityId = la.LiabilityId
                    }).ToList();

                    await _baseLiabilityAssetRepository.CreateAsync(liabilityAssets);
                }
            }

            return await GetAssetByIdAsync(asset.Id);
        }

        public async Task DeleteAssetAsync(Guid id)
        {
            var asset = await _baseAssetRepository.GetByIdAsync(id);
            if (asset == null)
            {
                throw new ResponseErrorObject("Asset not found", StatusCodes.Status404NotFound);
            }

            await _baseAssetRepository.DeleteAsync(id);
        }
    }
}
