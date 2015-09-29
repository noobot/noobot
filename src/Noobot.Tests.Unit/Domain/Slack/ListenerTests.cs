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
            LoginResponse loginResponsey;

            // when
            client.Connect(loginResponse =>
            {
                //This is called once the client has emitted the RTM start command
                loginResponsey = loginResponse;

            }, () =>
            {
                //This is called once the Real Time Messaging client has connected to the end point

                client.OnMessageReceived += delegate (NewMessage message)
                {
                    Console.WriteLine(message.text);
                    var response = new NewMessage
                    {
                        channel = message.channel,
                        user = message.user,
                        text = "Hi - TEST"
                    };

                    //client.Message(response);
                    client.SendMessage(messageReceived =>
                    {
                        response.channel = messageReceived.text;

                    }, message.channel, "HI - MEGA TEST");
                };

            });

            // then
            for (int i = 0; i < 10000; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
    }
}