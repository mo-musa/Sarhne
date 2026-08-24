using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Errors;

public record Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}
