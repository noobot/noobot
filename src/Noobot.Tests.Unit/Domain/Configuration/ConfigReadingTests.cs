using Noobot.Domain.Configuration;
using NUnit.Framework;
using Should;

namespace Noobot.Tests.Unit.Domain.Configuration
{
    [TestFixture]
    public class ConfigReadingTests
    {
        [Test]
        public void should_read_config()
        {
            // given

            // when
            var config = Config.GetConfig();

            // then
            config.ShouldNotBeNull();
            config.Slack.ShouldNotBeNull();
            config.Slack.ApiToken.ShouldNotBeNull();
        }
    }
}