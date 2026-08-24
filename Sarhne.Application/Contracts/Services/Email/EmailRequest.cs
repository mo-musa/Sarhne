using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Email;

public sealed record EmailRequest(
    string To,
    string Subject,
    string Body,
    bool IsHtml = true);