using MediatR;
using Microsoft.AspNetCore.Identity;
using Sarhne.Application.Common.Results;
using Sarhne.Domain.Constants;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.SuperAdmin.Role.GetAdmins;

public sealed class GetAdminsQueryHandler
    : IRequestHandler<
        GetAdminsQuery,
        Result<IReadOnlyList<AdminDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAdminsQueryHandler(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IReadOnlyList<AdminDto>>> Handle(
        GetAdminsQuery request,
        CancellationToken cancellationToken)
    {
        var admins =
            await _userManager.GetUsersInRoleAsync(
                Roles.Admin);

        var result = admins
            .Select(x => new AdminDto
            {
                Id = x.Id,
                UserName = x.UserName!,
                Email = x.Email,
                FullName = x.FullName,
                ImageUrl = x.ImageUrl,
                CreatedAt = x.CreatedAt
            })
            .ToList();

        return result;
    }
}