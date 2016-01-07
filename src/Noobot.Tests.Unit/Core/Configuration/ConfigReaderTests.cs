using Noobot.Core.Configuration;
using Noobot.Runner.Configuration;
using NUnit.Framework;
using Should;

namespace Noobot.Tests.Unit.Core.Configuration
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
            string slackKey = reader.SlackApiKey();

            // then
            slackKey.ShouldNotBeNull();
        }
    }
}