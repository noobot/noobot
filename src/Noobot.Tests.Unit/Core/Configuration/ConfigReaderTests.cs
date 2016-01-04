using Noobot.Core.Configuration;
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
            var config = reader.GetConfig();

            // then
            config.ShouldNotBeNull();
            config["slack"].Value<string>("apiToken").ShouldNotBeNull();
        }
    }
}