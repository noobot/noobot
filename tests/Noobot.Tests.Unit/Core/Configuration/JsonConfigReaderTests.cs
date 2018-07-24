﻿using Noobot.Core.Configuration;
using Shouldly;
using Xunit;

namespace Noobot.Tests.Unit.Core.Configuration
{
    public class JsonConfigReaderTests
    {
        [Fact]
        public void should_read_config()
        {
            // given
            var reader = JsonConfigReader.DefaultLocation();

            // when
            string slackKey = reader.SlackApiKey;

            // then
            slackKey.ShouldNotBeNull();
        }
    }
}