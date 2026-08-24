using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Me;

public sealed class MeQueryHandler
    : IRequestHandler<MeQuery, Result<MeResponse>>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public MeQueryHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MeResponse>> Handle(
        MeQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated ||
            _currentUserService.UserId is null)
        {
            return AuthenticationErrors.Unauthorized;
        }

        var user = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == _currentUserService.UserId)
            .Select(x => new
            {
                x.Id,
                x.UserName,
                x.FullName,
                x.Email
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return AuthenticationErrors.Unauthorized;
        }

        var roles = await (
            from userRole in _context.UserRoles
            join role in _context.Roles
                on userRole.RoleId equals role.Id
            where userRole.UserId == user.Id
            select role.Name!
        ).ToListAsync(cancellationToken);

        return new MeResponse(
            user.Id,
            user.UserName!,
            user.FullName!,
            user.Email!,
            roles);
    }
}