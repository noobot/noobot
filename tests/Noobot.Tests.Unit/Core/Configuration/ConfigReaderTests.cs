using Noobot.Core.Configuration;
using PowerAssert;
using Xunit;

namespace Noobot.Tests.Unit.Core.Configuration
{
    public class ConfigReaderTests
    {
        [Fact]
        public void should_read_config()
        {
            // given
            var reader = new ConfigReader();

            // when
            string slackKey = reader.SlackApiKey;

            // then
            PAssert.IsTrue(() => slackKey != null);
        }
    }
}