using MediatR;
using Microsoft.AspNetCore.Http;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.SendMessage;

public sealed record SendMessageCommand(
    string UserName,
    string? Content,
    IFormFile? Photo,
    bool IsAnonymous) : IRequest<Result>;