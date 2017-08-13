using Noobot.Core.Configuration;
using Should;
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
            slackKey.ShouldNotBeNull();
        }
    }
}