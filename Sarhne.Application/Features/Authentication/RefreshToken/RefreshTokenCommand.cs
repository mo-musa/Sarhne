using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.RefreshToken;

public sealed record RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>;
