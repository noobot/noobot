using SlackConnector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noobot.Core
{
    public static class Extensions
    {
        public static IReadOnlyDictionary<string, SlackUser> WithEmailSet(this IReadOnlyDictionary<string, SlackUser> userCache) => 
            new ReadOnlyDictionary<string, SlackUser>(userCache.Where(x => x.Value.Email != null).ToDictionary(z => z.Key, z => z.Value));

        public static SlackUser FindByEmail(this IReadOnlyDictionary<string, SlackUser> userCache, string email) =>
            userCache.Where(x => x.Value.Email != null).FirstOrDefault(x => x.Value.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).Value;
    }
}
