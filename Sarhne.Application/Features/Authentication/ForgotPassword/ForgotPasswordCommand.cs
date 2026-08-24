using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Authentication.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;