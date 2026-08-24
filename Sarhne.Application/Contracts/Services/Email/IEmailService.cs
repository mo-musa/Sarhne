using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Email;

public interface IEmailService
{
    Task SendAsync(
        EmailRequest message,
        CancellationToken cancellationToken = default);
}