using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Loan;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.Loan;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Shared.Models;

namespace Zamm.Application.ImplementService;

public class LoanService : ILoanService
{
    private readonly IBaseRepository<Loan> _baseLoanRepository;
    private readonly IBaseRepository<Lender> _baseLenderRepository;
    private readonly IBaseRepository<InterestRate> _baseInterestRateRepository;
    private readonly IBaseRepository<Liability> _baseLiabilityRepository;

    public LoanService(
        IBaseRepository<Loan> baseLoanRepository,
        IBaseRepository<Lender> baseLenderRepository,
        IBaseRepository<InterestRate> baseInterestRateRepository,
        IBaseRepository<Liability> baseLiabilityRepository)
    {
        _baseLoanRepository = baseLoanRepository;
        _baseLenderRepository = baseLenderRepository;
        _baseInterestRateRepository = baseInterestRateRepository;
        _baseLiabilityRepository = baseLiabilityRepository;
    }

    public async Task<PagedResult<LoanResult>> GetListLoanAsync(LoanQuery loanQuery)
    {
        var query = _baseLoanRepository.BuildQueryable(
            new List<string> { "Lender", "InterestRates", "Liabilities" },
            null
        );

        if (!string.IsNullOrEmpty(loanQuery.Name))
        {
            var name = loanQuery.Name.ToLower();
            query = query.Where(l => l.Name.ToLower().Contains(name));
        }

        if (loanQuery.LenderId.HasValue)
        {
            query = query.Where(l => l.LenderId == loanQuery.LenderId.Value);
        }

        var totalCount = await query.CountAsync();

        query = _baseLoanRepository.ApplySorting(query, loanQuery.SortBy, loanQuery.SortDescending);

        var results = await query
            .Skip((loanQuery.PageNumber - 1) * loanQuery.PageSize)
            .Take(loanQuery.PageSize)
            .Select(LoanResult.FromLoan)
            .ToListAsync();

        return new PagedResult<LoanResult>(results, totalCount, loanQuery.PageNumber, loanQuery.PageSize);
    }

    public async Task<LoanResult> GetLoanByIdAsync(Guid id)
    {
        var loan = await _baseLoanRepository
            .BuildQueryable(
                new List<string> { "Lender", "InterestRates", "Liabilities" },
                l => l.Id == id
            )
            .Select(LoanResult.FromLoan)
            .FirstOrDefaultAsync();

        if (loan == null)
        {
            throw new ResponseErrorObject("Loan not found", StatusCodes.Status404NotFound);
        }

        return loan;
    }

    public async Task<LoanResult> CreateLoanAsync(CreateLoanInput request)
    {
        var lenderExists = await _baseLenderRepository.GetByIdAsync(request.LenderId);
        if (lenderExists == null)
        {
            throw new ResponseErrorObject("Lender not found", StatusCodes.Status400BadRequest);
        }

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            LenderId = request.LenderId,
            UpdatedAt = DateTime.UtcNow
        };

        await _baseLoanRepository.CreateAsync(loan);

        if (request.InterestRates != null && request.InterestRates.Any())
        {
            var interestRates = request.InterestRates.Select(ir => new InterestRate
            {
                Id = Guid.NewGuid(),
                LoanId = loan.Id,
                RateType = ir.RateType,
                Rate = ir.Rate,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _baseInterestRateRepository.CreateAsync(interestRates);
        }

        return await GetLoanByIdAsync(loan.Id);
    }

    public async Task<LoanResult> UpdateLoanAsync(Guid loanId, UpdateLoanInput request)
    {
        var loan = await _baseLoanRepository.GetByIdAsync(loanId);
        if (loan == null)
        {
            throw new ResponseErrorObject("Loan not found", StatusCodes.Status404NotFound);
        }

        var lenderExists = await _baseLenderRepository.GetByIdAsync(request.LenderId);
        if (lenderExists == null)
        {
            throw new ResponseErrorObject("Lender not found", StatusCodes.Status400BadRequest);
        }

        loan.Name = request.Name;
        loan.LenderId = request.LenderId;
        loan.UpdatedAt = DateTime.UtcNow;

        await _baseLoanRepository.UpdateAsync(loan);

        if (request.InterestRates != null)
        {
            await _baseInterestRateRepository.DeleteAsync(ir => ir.LoanId == loanId);

            if (request.InterestRates.Any())
            {
                var interestRates = request.InterestRates.Select(ir => new InterestRate
                {
                    Id = Guid.NewGuid(),
                    LoanId = loan.Id,
                    RateType = ir.RateType,
                    Rate = ir.Rate,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _baseInterestRateRepository.CreateAsync(interestRates);
            }
        }

        return await GetLoanByIdAsync(loan.Id);
    }

    public async Task DeleteLoanAsync(Guid id)
    {
        var loan = await _baseLoanRepository.GetByIdAsync(id);
        if (loan == null)
        {
            throw new ResponseErrorObject("Loan not found", StatusCodes.Status404NotFound);
        }

        var hasLiabilities = await _baseLiabilityRepository.GetAsync(l => l.LoanId == id);
        if (hasLiabilities != null)
        {
            throw new ResponseErrorObject("Cannot delete loan with associated liabilities. Please delete all liabilities first.", StatusCodes.Status400BadRequest);
        }

        await _baseLoanRepository.DeleteAsync(id);
    }
}