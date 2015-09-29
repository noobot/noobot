using System;
using System.Linq;
using System.Threading;
using Noobot.Domain.Configuration;
using NUnit.Framework;
using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace Noobot.Tests.Unit.Domain.Slack
{
    [TestFixture]
    public class ListenerTests
    {
        [Test]
        public void should_connect_and_do_stuff()
        {
            // given
            Config config = new ConfigReader().GetConfig();
            var client = new SlackSocketClient(config.Slack.ApiToken);
            
            // when
            client.Connect(loginResponse =>
            {
                //This is called once the client has emitted the RTM start command
                var bots = loginResponse.bots;
                var user = loginResponse.users.FirstOrDefault(x => x.id.Equals("U025U3V33", StringComparison.InvariantCultureIgnoreCase));
            }, () => {
                //This is called once the RTM client has connected to the end point
               config = config;
            });

            client.OnMessageReceived += delegate(NewMessage message)
            {
                Console.WriteLine(message.text);
            };

            // then
            for (int i = 0; i < 10000; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
    }
}