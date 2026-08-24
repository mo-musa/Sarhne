using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.SuperAdmin.Role.AddRoleToUser;

public sealed record AddRoleToUserCommand(
    int UserId,
    string Role) : IRequest<Result>;