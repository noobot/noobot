using SlackConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Noobot.Core
{
    public static class Extensions
    {
        public static IReadOnlyDictionary<string, SlackUser> WithEmailSet(
            this IReadOnlyDictionary<string, SlackUser> userCache)
        {
            return userCache.Where(x => x.Value.Email != null)
                .ToDictionary(z => z.Key, z => z.Value);
        }

        public static SlackUser FindByEmail(
            this IReadOnlyDictionary<string, SlackUser> userCache,
            string email)
        {
            return userCache
                .FirstOrDefault(x => (x.Value.Email ?? "").Equals(email, StringComparison.OrdinalIgnoreCase)).Value;
        }
    }
}