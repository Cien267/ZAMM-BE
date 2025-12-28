using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Lender;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.Lender;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Shared.Models;

namespace Zamm.Application.ImplementService;

public class LenderService : ILenderService
{
    private readonly IBaseRepository<Lender> _baseLenderRepository;
    private readonly IBaseRepository<Loan> _baseLoanRepository;

    public LenderService(
        IBaseRepository<Lender> baseLenderRepository,
        IBaseRepository<Loan> baseLoanRepository)
    {
        _baseLenderRepository = baseLenderRepository;
        _baseLoanRepository = baseLoanRepository;
    }

    public async Task<PagedResult<LenderResult>> GetListLenderAsync(LenderQuery lenderQuery)
    {
        var query = _baseLenderRepository.BuildQueryable(
            new List<string> { "Loans.InterestRates", "Loans.Liabilities" },
            null
        );

        if (!string.IsNullOrEmpty(lenderQuery.Name))
        {
            var name = lenderQuery.Name.ToLower();
            query = query.Where(l => l.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrEmpty(lenderQuery.Slug))
        {
            var slug = lenderQuery.Slug.ToLower();
            query = query.Where(l => l.Slug.ToLower().Contains(slug));
        }

        var totalCount = await query.CountAsync();

        query = _baseLenderRepository.ApplySorting(query, lenderQuery.SortBy, lenderQuery.SortDescending);

        var results = await query
            .Skip((lenderQuery.PageNumber - 1) * lenderQuery.PageSize)
            .Take(lenderQuery.PageSize)
            .Select(LenderResult.FromLender)
            .ToListAsync();

        return new PagedResult<LenderResult>(results, totalCount, lenderQuery.PageNumber, lenderQuery.PageSize);
    }

    public async Task<LenderResult> GetLenderByIdAsync(Guid id)
    {
        var lender = await _baseLenderRepository
            .BuildQueryable(
                new List<string> { "Loans.InterestRates", "Loans.Liabilities" },
                l => l.Id == id
            )
            .Select(LenderResult.FromLender)
            .FirstOrDefaultAsync();

        if (lender == null)
        {
            throw new ResponseErrorObject("Lender not found", StatusCodes.Status404NotFound);
        }

        return lender;
    }

    public async Task<LenderResult> CreateLenderAsync(CreateLenderInput request)
    {
        var slugExists = await _baseLenderRepository.GetAsync(l => l.Slug == request.Slug);
        if (slugExists != null)
        {
            throw new ResponseErrorObject("Slug already exists", StatusCodes.Status400BadRequest);
        }

        var lender = new Lender
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug
        };

        await _baseLenderRepository.CreateAsync(lender);

        return await GetLenderByIdAsync(lender.Id);
    }

    public async Task<LenderResult> UpdateLenderAsync(Guid lenderId, UpdateLenderInput request)
    {
        var lender = await _baseLenderRepository.GetByIdAsync(lenderId);
        if (lender == null)
        {
            throw new ResponseErrorObject("Lender not found", StatusCodes.Status404NotFound);
        }

        if (lender.Slug != request.Slug)
        {
            var slugExists = await _baseLenderRepository.GetAsync(l => l.Slug == request.Slug && l.Id != lenderId);
            if (slugExists != null)
            {
                throw new ResponseErrorObject("Slug already exists", StatusCodes.Status400BadRequest);
            }
        }

        lender.Name = request.Name;
        lender.Slug = request.Slug;

        await _baseLenderRepository.UpdateAsync(lender);

        return await GetLenderByIdAsync(lender.Id);
    }

    public async Task DeleteLenderAsync(Guid id)
    {
        var lender = await _baseLenderRepository.GetByIdAsync(id);
        if (lender == null)
        {
            throw new ResponseErrorObject("Lender not found", StatusCodes.Status404NotFound);
        }

        var hasLoans = await _baseLoanRepository.GetAsync(l => l.LenderId == id);
        if (hasLoans != null)
        {
            throw new ResponseErrorObject("Cannot delete lender with associated loans. Please delete or reassign all loans first.", StatusCodes.Status400BadRequest);
        }

        await _baseLenderRepository.DeleteAsync(id);
    }
}