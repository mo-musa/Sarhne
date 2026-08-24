using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Errors;

//public enum ErrorType
//{
//    None = 0,
//    Validation = 1,
//    NotFound = 2,
//    Conflict = 3,
//    Unauthorized = 4,
//    Forbidden = 5,
//    Unexpected = 6
//}
public enum ErrorType
{
    None = 200,
    Validation = 400,
    NotFound = 404,
    Conflict = 409,
    Unauthorized = 401,
    Forbidden = 403,
    Unexpected = 500
}
