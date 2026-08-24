using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.Me;

public sealed record MeQuery()
    : IRequest<Result<MeResponse>>;