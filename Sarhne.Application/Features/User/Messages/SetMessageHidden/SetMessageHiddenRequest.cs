using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Features.User.Messages.SetMessageHidden;

public sealed record SetMessageHiddenRequest(
    bool IsHidden);