using System.Threading.Tasks;
using Noobot.Domain.Configuration;
using Noobot.Domain.Slack;
using NUnit.Framework;

namespace Noobot.Tests.Unit.Domain.Slack
{
    [TestFixture]
    public class SlackConnectorTests
    {
        [Test]
        public void should_connect_as_expected()
        {
            // given
            var configReader = new ConfigReader();
            var connector = new SlackConnector(configReader);

            // when
            var task = connector.Connect();
            task.Wait();

            // then
            Assert.That(task.Result, Is.Not.Null);
        }
    }
}