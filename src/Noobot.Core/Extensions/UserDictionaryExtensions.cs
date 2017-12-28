using System;
using System.Collections.Generic;
using System.Linq;
using SlackConnector.Models;

namespace Noobot.Core.Extensions
{
    internal static class UserDictionaryExtensions
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