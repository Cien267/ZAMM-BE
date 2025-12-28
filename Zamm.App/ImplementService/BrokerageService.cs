using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Brokerage;
using Zamm.Application.Payloads.InputModels.Invitation;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.Brokerage;
using Zamm.Application.Payloads.ResultModels.Invitation;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Shared.Models;

namespace Zamm.Application.ImplementService;

public class BrokerageService : IBrokerageService
{
    private readonly IBaseRepository<Brokerage> _baseBrokerageRepository;
    private readonly IBaseRepository<BrokerageLogo> _baseBrokerageLogoRepository;
    private readonly IBaseRepository<Invitation> _baseInvitationRepository;
    private readonly IBaseRepository<User> _baseUserRepository;

    public BrokerageService(
        IBaseRepository<Brokerage> baseBrokerageRepository,
        IBaseRepository<BrokerageLogo> baseBrokerageLogoRepository,
        IBaseRepository<Invitation> baseInvitationRepository,
        IBaseRepository<User> baseUserRepository)
    {
        _baseBrokerageRepository = baseBrokerageRepository;
        _baseBrokerageLogoRepository = baseBrokerageLogoRepository;
        _baseInvitationRepository = baseInvitationRepository;
        _baseUserRepository = baseUserRepository;
    }

    public async Task<PagedResult<BrokerageResult>> GetListBrokerageAsync(BrokerageQuery brokerageQuery)
    {
        var query = _baseBrokerageRepository.BuildQueryable(
            new List<string> { "Logos", "Users", "Invitations" },
            null
        );

        if (!string.IsNullOrEmpty(brokerageQuery.Name))
        {
            var name = brokerageQuery.Name.ToLower();
            query = query.Where(b => b.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrEmpty(brokerageQuery.Slug))
        {
            var slug = brokerageQuery.Slug.ToLower();
            query = query.Where(b => b.Slug.ToLower().Contains(slug));
        }

        if (brokerageQuery.IsMasterAccount.HasValue)
        {
            query = query.Where(b => b.IsMasterAccount == brokerageQuery.IsMasterAccount.Value);
        }

        var totalCount = await query.CountAsync();

        query = _baseBrokerageRepository.ApplySorting(query, brokerageQuery.SortBy, brokerageQuery.SortDescending);

        var results = await query
            .Skip((brokerageQuery.PageNumber - 1) * brokerageQuery.PageSize)
            .Take(brokerageQuery.PageSize)
            .Select(BrokerageResult.FromBrokerage)
            .ToListAsync();

        return new PagedResult<BrokerageResult>(results, totalCount, brokerageQuery.PageNumber, brokerageQuery.PageSize);
    }

    public async Task<BrokerageResult> GetBrokerageByIdAsync(Guid id)
    {
        var brokerage = await _baseBrokerageRepository
            .BuildQueryable(
                new List<string> { "Logos", "Users", "Invitations" },
                b => b.Id == id
            )
            .Select(BrokerageResult.FromBrokerage)
            .FirstOrDefaultAsync();

        if (brokerage == null)
        {
            throw new ResponseErrorObject("Brokerage not found", StatusCodes.Status404NotFound);
        }

        return brokerage;
    }

    public async Task<BrokerageResult> CreateBrokerageAsync(CreateBrokerageInput request)
    {
        var slugExists = await _baseBrokerageRepository.GetAsync(b => b.Slug == request.Slug);
        if (slugExists != null)
        {
            throw new ResponseErrorObject("Slug already exists", StatusCodes.Status400BadRequest);
        }

        if (!string.IsNullOrEmpty(request.AuthorisedDomain))
        {
            var domainExists = await _baseBrokerageRepository.GetAsync(b => b.AuthorisedDomain == request.AuthorisedDomain);
            if (domainExists != null)
            {
                throw new ResponseErrorObject("Authorised domain already in use by another brokerage", StatusCodes.Status400BadRequest);
            }
        }

        var brokerage = new Brokerage
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            AuthorisedDomain = request.AuthorisedDomain,
            IsMasterAccount = request.IsMasterAccount,
            CreatedAt = DateTime.UtcNow
        };

        await _baseBrokerageRepository.CreateAsync(brokerage);

        if (request.Logos != null && request.Logos.Any())
        {
            var logos = request.Logos.Select(l => new BrokerageLogo
            {
                Id = Guid.NewGuid(),
                BrokerageId = brokerage.Id,
                Url = l.Url
            }).ToList();

            await _baseBrokerageLogoRepository.CreateAsync(logos);
        }

        return await GetBrokerageByIdAsync(brokerage.Id);
    }

    public async Task<BrokerageResult> UpdateBrokerageAsync(Guid brokerageId, UpdateBrokerageInput request)
    {
        var brokerage = await _baseBrokerageRepository.GetByIdAsync(brokerageId);
        if (brokerage == null)
        {
            throw new ResponseErrorObject("Brokerage not found", StatusCodes.Status404NotFound);
        }

        if (brokerage.Slug != request.Slug)
        {
            var slugExists = await _baseBrokerageRepository.GetAsync(b => b.Slug == request.Slug && b.Id != brokerageId);
            if (slugExists != null)
            {
                throw new ResponseErrorObject("Slug already exists", StatusCodes.Status400BadRequest);
            }
        }

        if (!string.IsNullOrEmpty(request.AuthorisedDomain) && brokerage.AuthorisedDomain != request.AuthorisedDomain)
        {
            var domainExists = await _baseBrokerageRepository.GetAsync(b => b.AuthorisedDomain == request.AuthorisedDomain && b.Id != brokerageId);
            if (domainExists != null)
            {
                throw new ResponseErrorObject("Authorised domain already in use by another brokerage", StatusCodes.Status400BadRequest);
            }
        }

        brokerage.Name = request.Name;
        brokerage.Slug = request.Slug;
        brokerage.AuthorisedDomain = request.AuthorisedDomain;
        brokerage.IsMasterAccount = request.IsMasterAccount;

        await _baseBrokerageRepository.UpdateAsync(brokerage);

        if (request.Logos != null)
        {
            await _baseBrokerageLogoRepository.DeleteAsync(l => l.BrokerageId == brokerageId);

            if (request.Logos.Any())
            {
                var logos = request.Logos.Select(l => new BrokerageLogo
                {
                    Id = Guid.NewGuid(),
                    BrokerageId = brokerage.Id,
                    Url = l.Url
                }).ToList();

                await _baseBrokerageLogoRepository.CreateAsync(logos);
            }
        }

        return await GetBrokerageByIdAsync(brokerage.Id);
    }

    public async Task DeleteBrokerageAsync(Guid id)
    {
        var brokerage = await _baseBrokerageRepository.GetByIdAsync(id);
        if (brokerage == null)
        {
            throw new ResponseErrorObject("Brokerage not found", StatusCodes.Status404NotFound);
        }

        var hasUsers = await _baseUserRepository.GetAsync(u => u.BrokerageId == id);
        if (hasUsers != null)
        {
            throw new ResponseErrorObject("Cannot delete brokerage with associated users. Please delete or reassign all users first.", StatusCodes.Status400BadRequest);
        }

        await _baseBrokerageRepository.DeleteAsync(id);
    }

    public async Task<InvitationResult> CreateInvitationAsync(CreateInvitationInput request)
    {
        var brokerageExists = await _baseBrokerageRepository.GetByIdAsync(request.BrokerageId);
        if (brokerageExists == null)
        {
            throw new ResponseErrorObject("Brokerage not found", StatusCodes.Status400BadRequest);
        }

        var inviterExists = await _baseUserRepository.GetByIdAsync(request.InviterId);
        if (inviterExists == null)
        {
            throw new ResponseErrorObject("Inviter not found", StatusCodes.Status400BadRequest);
        }

        var existingInvitation = await _baseInvitationRepository.GetAsync(i => 
            i.Email == request.Email && 
            i.BrokerageId == request.BrokerageId && 
            !i.IsComplete);
        
        if (existingInvitation != null)
        {
            throw new ResponseErrorObject("An active invitation already exists for this email", StatusCodes.Status400BadRequest);
        }

        var existingUser = await _baseUserRepository.GetAsync(u => 
            u.Email == request.Email && 
            u.BrokerageId == request.BrokerageId);
        
        if (existingUser != null)
        {
            throw new ResponseErrorObject("User with this email already exists in this brokerage", StatusCodes.Status400BadRequest);
        }

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            IsComplete = false,
            IsAdmin = request.IsAdmin,
            BrokerageId = request.BrokerageId,
            InviterId = request.InviterId,
            CreatedAt = DateTime.UtcNow
        };

        await _baseInvitationRepository.CreateAsync(invitation);

        var result = await _baseInvitationRepository
            .BuildQueryable(
                new List<string> { "Brokerage", "Inviter" },
                i => i.Id == invitation.Id
            )
            .Select(InvitationResult.FromInvitation)
            .FirstOrDefaultAsync();

        return result!;
    }

    public async Task<List<InvitationResult>> GetInvitationsByBrokerageIdAsync(Guid brokerageId)
    {
        var invitations = await _baseInvitationRepository
            .BuildQueryable(
                new List<string> { "Brokerage", "Inviter" },
                i => i.BrokerageId == brokerageId
            )
            .Select(InvitationResult.FromInvitation)
            .ToListAsync();

        return invitations;
    }

    public async Task DeleteInvitationAsync(Guid id)
    {
        var invitation = await _baseInvitationRepository.GetByIdAsync(id);
        if (invitation == null)
        {
            throw new ResponseErrorObject("Invitation not found", StatusCodes.Status404NotFound);
        }

        await _baseInvitationRepository.DeleteAsync(id);
    }
}