using MediatR;
using Microsoft.EntityFrameworkCore;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Users.DeleteUser;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly ISarhneDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public DeleteUserCommandHandler(
        ISarhneDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == _currentUser.UserId)
            return UserErrors.CannotDeleteYourself;

        var targetUser = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.UserId)
            .Select(u => new
            {
                Entity = u,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (targetUser is null)
            return UserErrors.UserNotFound;

        var isTargetAdmin = targetUser.Roles.Contains(Roles.Admin);
        var isTargetSuperAdmin = targetUser.Roles.Contains(Roles.SuperAdmin);

        var isCurrentSuperAdmin = _currentUser.Roles.Contains(Roles.SuperAdmin);

        if (isTargetSuperAdmin || (!isCurrentSuperAdmin && isTargetAdmin))
            return UserErrors.CannotDeleteAdmin;


        _context.Users.Remove(targetUser.Entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
