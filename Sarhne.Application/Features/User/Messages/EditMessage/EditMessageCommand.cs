using MediatR;
using Microsoft.AspNetCore.Http;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.EditMessage;

public sealed record EditMessageCommand(
    int MessageId,
    string? Content,
    IFormFile? Photo,
    bool RemoveContent,
    bool RemovePhoto) : IRequest<Result>;