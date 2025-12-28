using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Address;
using Zamm.Application.Payloads.InputModels.Asset;
using Zamm.Application.Payloads.InputModels.Liability;
using Zamm.Application.Payloads.Responses;
using Zamm.Shared.Models;
using Zamm.Application.Payloads.ResultModels.Asset;
using Zamm.Application.Payloads.ResultModels.Liability;

namespace Zamm.Application.ImplementService
{
    public class LiabilityService : ILiabilityService
    {
        private readonly IBaseRepository<Liability> _baseLiabilityRepository;
        private readonly IBaseRepository<Loan> _baseLoanRepository;
        private readonly IBaseRepository<Person> _basePersonRepository;
        private readonly IBaseRepository<Company> _baseCompanyRepository;
        private readonly IBaseRepository<Asset> _baseAssetRepository;
        private readonly IBaseRepository<LiabilityPerson> _baseLiabilityPersonRepository;
        private readonly IBaseRepository<LiabilityCompany> _baseLiabilityCompanyRepository;
        private readonly IBaseRepository<LiabilityAsset> _baseLiabilityAssetRepository;
        private readonly IBaseRepository<FixedRatePeriod> _baseFixedRatePeriodRepository;

        public LiabilityService(
            IBaseRepository<Liability> baseLiabilityRepository,
            IBaseRepository<Loan> baseLoanRepository,
            IBaseRepository<Person> basePersonRepository,
            IBaseRepository<Company> baseCompanyRepository,
            IBaseRepository<Asset> baseAssetRepository,
            IBaseRepository<LiabilityPerson> baseLiabilityPersonRepository,
            IBaseRepository<LiabilityCompany> baseLiabilityCompanyRepository,
            IBaseRepository<LiabilityAsset> baseLiabilityAssetRepository,
            IBaseRepository<FixedRatePeriod> baseFixedRatePeriodRepository)
        {
            _baseLiabilityRepository = baseLiabilityRepository;
            _baseLoanRepository = baseLoanRepository;
            _basePersonRepository = basePersonRepository;
            _baseCompanyRepository = baseCompanyRepository;
            _baseAssetRepository = baseAssetRepository;
            _baseLiabilityPersonRepository = baseLiabilityPersonRepository;
            _baseLiabilityCompanyRepository = baseLiabilityCompanyRepository;
            _baseLiabilityAssetRepository = baseLiabilityAssetRepository;
            _baseFixedRatePeriodRepository = baseFixedRatePeriodRepository;
        }

        public async Task<PagedResult<LiabilityResult>> GetListLiabilityAsync(LiabilityQuery liabilityQuery)
        {
            var query = _baseLiabilityRepository.BuildQueryable(
                new List<string>
                {
                    "Loan.Lender",
                    "LiabilityPeople.Person",
                    "LiabilityCompanies.Company",
                    "LiabilityAssets.Asset",
                    "FixedRatePeriods"
                },
                null
            );

            if (!string.IsNullOrEmpty(liabilityQuery.Name))
            {
                var name = liabilityQuery.Name.ToLower();
                query = query.Where(l => l.Name != null && l.Name.ToLower().Contains(name));
            }

            if (liabilityQuery.LoanId.HasValue)
            {
                query = query.Where(l => l.LoanId == liabilityQuery.LoanId.Value);
            }

            if (!string.IsNullOrEmpty(liabilityQuery.FinancePurpose))
            {
                var purpose = liabilityQuery.FinancePurpose.ToLower();
                query = query.Where(l => l.FinancePurpose != null && l.FinancePurpose.ToLower().Contains(purpose));
            }

            if (liabilityQuery.StartDateFrom.HasValue)
            {
                query = query.Where(l => l.StartDate >= liabilityQuery.StartDateFrom.Value);
            }

            if (liabilityQuery.StartDateTo.HasValue)
            {
                query = query.Where(l => l.StartDate <= liabilityQuery.StartDateTo.Value);
            }

            var totalCount = await query.CountAsync();

            query = _baseLiabilityRepository.ApplySorting(query, liabilityQuery.SortBy, liabilityQuery.SortDescending);

            var results = await query
                .Skip((liabilityQuery.PageNumber - 1) * liabilityQuery.PageSize)
                .Take(liabilityQuery.PageSize)
                .Select(LiabilityResult.FromLiability)
                .ToListAsync();

            return new PagedResult<LiabilityResult>(results, totalCount, liabilityQuery.PageNumber,
                liabilityQuery.PageSize);
        }

        public async Task<LiabilityResult> GetLiabilityByIdAsync(Guid id)
        {
            var liability = await _baseLiabilityRepository
                .BuildQueryable(
                    new List<string>
                    {
                        "Loan.Lender",
                        "LiabilityPeople.Person",
                        "LiabilityCompanies.Company",
                        "LiabilityAssets.Asset",
                        "FixedRatePeriods"
                    },
                    l => l.Id == id
                )
                .Select(LiabilityResult.FromLiability)
                .FirstOrDefaultAsync();

            if (liability == null)
            {
                throw new ResponseErrorObject("Liability not found", StatusCodes.Status404NotFound);
            }

            return liability;
        }

        public async Task<LiabilityResult> CreateLiabilityAsync(CreateLiabilityInput request)
        {
            var loanExists = await _baseLoanRepository.GetByIdAsync(request.LoanId);
            if (loanExists == null)
            {
                throw new ResponseErrorObject("Loan not found", StatusCodes.Status400BadRequest);
            }

            if (request.LiabilityPeople != null && request.LiabilityPeople.Any())
            {
                foreach (var lp in request.LiabilityPeople)
                {
                    var personExists = await _basePersonRepository.GetByIdAsync(lp.PersonId);
                    if (personExists == null)
                    {
                        throw new ResponseErrorObject($"Person with ID {lp.PersonId} not found", StatusCodes.Status400BadRequest);
                    }
                }

                var totalPersonPercent = request.LiabilityPeople.Sum(lp => lp.Percent);
                var totalCompanyPercent = request.LiabilityCompanies?.Sum(lc => lc.Percent) ?? 0;
                if (totalPersonPercent + totalCompanyPercent > 100)
                {
                    throw new ResponseErrorObject("Total liability percentage cannot exceed 100%", StatusCodes.Status400BadRequest);
                }
            }

            if (request.LiabilityCompanies != null && request.LiabilityCompanies.Any())
            {
                foreach (var lc in request.LiabilityCompanies)
                {
                    var companyExists = await _baseCompanyRepository.GetByIdAsync(lc.CompanyId);
                    if (companyExists == null)
                    {
                        throw new ResponseErrorObject($"Company with ID {lc.CompanyId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }

            if (request.LiabilityAssets != null && request.LiabilityAssets.Any())
            {
                foreach (var la in request.LiabilityAssets)
                {
                    var assetExists = await _baseAssetRepository.GetByIdAsync(la.AssetId);
                    if (assetExists == null)
                    {
                        throw new ResponseErrorObject($"Asset with ID {la.AssetId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }

            var liability = new Liability
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                LoanTerm = request.LoanTerm,
                InterestOnlyTerm = request.InterestOnlyTerm,
                StartDate = request.StartDate,
                FinancePurpose = request.FinancePurpose,
                Amount = request.Amount,
                InitialBalance = request.InitialBalance,
                IntroRateYears = request.IntroRateYears,
                IntroRatePercent = request.IntroRatePercent,
                RepaymentAmount = request.RepaymentAmount,
                RepaymentFrequency = request.RepaymentFrequency,
                DiscountPercent = request.DiscountPercent,
                SettlementRate = request.SettlementRate,
                BankAccountName = request.BankAccountName,
                BankAccountBsb = request.BankAccountBsb,
                BankAccountNumber = request.BankAccountNumber,
                OffsetAccountBsb = request.OffsetAccountBsb,
                OffsetAccountNumber = request.OffsetAccountNumber,
                LoanId = request.LoanId
            };

            await _baseLiabilityRepository.CreateAsync(liability);

            if (request.LiabilityPeople != null && request.LiabilityPeople.Any())
            {
                var liabilityPeople = request.LiabilityPeople.Select(lp => new LiabilityPerson
                {
                    Id = Guid.NewGuid(),
                    LiabilityId = liability.Id,
                    PersonId = lp.PersonId,
                    Percent = lp.Percent
                }).ToList();

                await _baseLiabilityPersonRepository.CreateAsync(liabilityPeople);
            }

            if (request.LiabilityCompanies != null && request.LiabilityCompanies.Any())
            {
                var liabilityCompanies = request.LiabilityCompanies.Select(lc => new LiabilityCompany
                {
                    Id = Guid.NewGuid(),
                    LiabilityId = liability.Id,
                    CompanyId = lc.CompanyId,
                    Percent = lc.Percent
                }).ToList();

                await _baseLiabilityCompanyRepository.CreateAsync(liabilityCompanies);
            }

            if (request.LiabilityAssets != null && request.LiabilityAssets.Any())
            {
                var liabilityAssets = request.LiabilityAssets.Select(la => new LiabilityAsset
                {
                    Id = Guid.NewGuid(),
                    LiabilityId = liability.Id,
                    AssetId = la.AssetId
                }).ToList();

                await _baseLiabilityAssetRepository.CreateAsync(liabilityAssets);
            }

            if (request.FixedRatePeriods != null && request.FixedRatePeriods.Any())
            {
                var fixedRatePeriods = request.FixedRatePeriods.Select(frp => new FixedRatePeriod
                {
                    Id = Guid.NewGuid(),
                    LiabilityId = liability.Id,
                    StartDate = frp.StartDate,
                    Term = frp.Term,
                    CustomRate = frp.CustomRate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                await _baseFixedRatePeriodRepository.CreateAsync(fixedRatePeriods);
            }

            return await GetLiabilityByIdAsync(liability.Id);
        }

        public async Task<LiabilityResult> UpdateLiabilityAsync(Guid liabilityId, UpdateLiabilityInput request)
        {
            var liability = await _baseLiabilityRepository.GetByIdAsync(liabilityId);
            if (liability == null)
            {
                throw new ResponseErrorObject("Liability not found", StatusCodes.Status404NotFound);
            }

            var loanExists = await _baseLoanRepository.GetByIdAsync(request.LoanId);
            if (loanExists == null)
            {
                throw new ResponseErrorObject("Loan not found", StatusCodes.Status400BadRequest);
            }

            if (request.LiabilityPeople != null && request.LiabilityPeople.Any())
            {
                foreach (var lp in request.LiabilityPeople)
                {
                    var personExists = await _basePersonRepository.GetByIdAsync(lp.PersonId);
                    if (personExists == null)
                    {
                        throw new ResponseErrorObject($"Person with ID {lp.PersonId} not found", StatusCodes.Status400BadRequest);
                    }
                }

                var totalPersonPercent = request.LiabilityPeople.Sum(lp => lp.Percent);
                var totalCompanyPercent = request.LiabilityCompanies?.Sum(lc => lc.Percent) ?? 0;
                if (totalPersonPercent + totalCompanyPercent > 100)
                {
                    throw new ResponseErrorObject("Total liability percentage cannot exceed 100%", StatusCodes.Status400BadRequest);
                }
            }

            if (request.LiabilityCompanies != null && request.LiabilityCompanies.Any())
            {
                foreach (var lc in request.LiabilityCompanies)
                {
                    var companyExists = await _baseCompanyRepository.GetByIdAsync(lc.CompanyId);
                    if (companyExists == null)
                    {
                        throw new ResponseErrorObject($"Company with ID {lc.CompanyId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }

            if (request.LiabilityAssets != null && request.LiabilityAssets.Any())
            {
                foreach (var la in request.LiabilityAssets)
                {
                    var assetExists = await _baseAssetRepository.GetByIdAsync(la.AssetId);
                    if (assetExists == null)
                    {
                        throw new ResponseErrorObject($"Asset with ID {la.AssetId} not found", StatusCodes.Status400BadRequest);
                    }
                }
            }

            liability.Name = request.Name;
            liability.LoanTerm = request.LoanTerm;
            liability.InterestOnlyTerm = request.InterestOnlyTerm;
            liability.StartDate = request.StartDate;
            liability.FinancePurpose = request.FinancePurpose;
            liability.Amount = request.Amount;
            liability.InitialBalance = request.InitialBalance;
            liability.IntroRateYears = request.IntroRateYears;
            liability.IntroRatePercent = request.IntroRatePercent;
            liability.RepaymentAmount = request.RepaymentAmount;
            liability.RepaymentFrequency = request.RepaymentFrequency;
            liability.DiscountPercent = request.DiscountPercent;
            liability.SettlementRate = request.SettlementRate;
            liability.BankAccountName = request.BankAccountName;
            liability.BankAccountBsb = request.BankAccountBsb;
            liability.BankAccountNumber = request.BankAccountNumber;
            liability.OffsetAccountBsb = request.OffsetAccountBsb;
            liability.OffsetAccountNumber = request.OffsetAccountNumber;
            liability.LoanId = request.LoanId;

            await _baseLiabilityRepository.UpdateAsync(liability);

            if (request.LiabilityPeople != null)
            {
                await _baseLiabilityPersonRepository.DeleteAsync(lp => lp.LiabilityId == liabilityId);

                if (request.LiabilityPeople.Any())
                {
                    var liabilityPeople = request.LiabilityPeople.Select(lp => new LiabilityPerson
                    {
                        Id = Guid.NewGuid(),
                        LiabilityId = liability.Id,
                        PersonId = lp.PersonId,
                        Percent = lp.Percent
                    }).ToList();

                    await _baseLiabilityPersonRepository.CreateAsync(liabilityPeople);
                }
            }

            if (request.LiabilityCompanies != null)
            {
                await _baseLiabilityCompanyRepository.DeleteAsync(lc => lc.LiabilityId == liabilityId);

                if (request.LiabilityCompanies.Any())
                {
                    var liabilityCompanies = request.LiabilityCompanies.Select(lc => new LiabilityCompany
                    {
                        Id = Guid.NewGuid(),
                        LiabilityId = liability.Id,
                        CompanyId = lc.CompanyId,
                        Percent = lc.Percent
                    }).ToList();

                    await _baseLiabilityCompanyRepository.CreateAsync(liabilityCompanies);
                }
            }

            if (request.LiabilityAssets != null)
            {
                await _baseLiabilityAssetRepository.DeleteAsync(la => la.LiabilityId == liabilityId);

                if (request.LiabilityAssets.Any())
                {
                    var liabilityAssets = request.LiabilityAssets.Select(la => new LiabilityAsset
                    {
                        Id = Guid.NewGuid(),
                        LiabilityId = liability.Id,
                        AssetId = la.AssetId
                    }).ToList();

                    await _baseLiabilityAssetRepository.CreateAsync(liabilityAssets);
                }
            }

            if (request.FixedRatePeriods != null)
            {
                await _baseFixedRatePeriodRepository.DeleteAsync(frp => frp.LiabilityId == liabilityId);

                if (request.FixedRatePeriods.Any())
                {
                    var fixedRatePeriods = request.FixedRatePeriods.Select(frp => new FixedRatePeriod
                    {
                        Id = Guid.NewGuid(),
                        LiabilityId = liability.Id,
                        StartDate = frp.StartDate,
                        Term = frp.Term,
                        CustomRate = frp.CustomRate,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }).ToList();

                    await _baseFixedRatePeriodRepository.CreateAsync(fixedRatePeriods);
                }
            }

            return await GetLiabilityByIdAsync(liability.Id);
        }

        public async Task DeleteLiabilityAsync(Guid id)
        {
            var liability = await _baseLiabilityRepository.GetByIdAsync(id);
            if (liability == null)
            {
                throw new ResponseErrorObject("Asset not found", StatusCodes.Status404NotFound);
            }

            await _baseLiabilityRepository.DeleteAsync(id);
        }
    }
}
