using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Common.Caching;

public static class CacheKeys
{
    public static string UserSettings(int userId)
        => $"user-settings:{userId}";
    public static string PublicMessages(
        int userId,
        int pageNumber,
        int pageSize)
        => $"user:{userId}:public-messages:page={pageNumber}:size={pageSize}";

    public static string PublicMessagesPrefix(int userId)
    => $"user:{userId}:public-messages:";
}