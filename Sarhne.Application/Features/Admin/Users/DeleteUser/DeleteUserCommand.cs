using MediatR;
using Sarhne.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.Admin.Users.DeleteUser;

public sealed record DeleteUserCommand(int UserId) : IRequest<Result>;