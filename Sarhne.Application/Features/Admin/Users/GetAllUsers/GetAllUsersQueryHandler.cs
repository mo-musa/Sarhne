using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Users.GetAllUsers;

public sealed class GetAllUsersQueryHandler
    : IRequestHandler<
        GetAllUsersQuery,
        Result<PagedResult<GetAllUsersDto>>>
{
    private readonly ISarhneDbContext _context;

    public GetAllUsersQueryHandler(
        ISarhneDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<GetAllUsersDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Users
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .Skip(
                (request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetAllUsersDto
            {
                Id = x.Id,
                UserName = x.UserName!,
                FullName = x.FullName,
                Gender = x.Gender,
                ImageUrl = x.ImageUrl,
                Roles = x.UserRoles.Select(ur=>ur.Role.Name!).ToList()
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<GetAllUsersDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return result;
    }
}