using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.SuperAdmin.Role.GetAdmins;

public sealed record GetAdminsQuery
    : IRequest<Result<IReadOnlyList<AdminDto>>>;