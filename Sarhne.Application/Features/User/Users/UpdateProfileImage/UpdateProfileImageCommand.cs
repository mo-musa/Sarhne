using MediatR;
using Microsoft.AspNetCore.Http;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Users.UpdateProfileImage;

public sealed record UpdateProfileImageCommand(
    IFormFile Image
) : IRequest<Result<string>>;