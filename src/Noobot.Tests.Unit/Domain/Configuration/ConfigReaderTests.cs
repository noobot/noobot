using Noobot.Domain.Configuration;
using NUnit.Framework;
using Should;

namespace Noobot.Tests.Unit.Domain.Configuration
{
    [TestFixture]
    public class ConfigReaderTests
    {
        [Test]
        public void should_read_config()
        {
            // given
            var reader = new ConfigReader();

            // when
            var config = reader.GetConfig();

            // then
            config.ShouldNotBeNull();
            config.Slack.ShouldNotBeNull();
            config.Slack.ApiToken.ShouldNotBeNull();
        }
    }
}