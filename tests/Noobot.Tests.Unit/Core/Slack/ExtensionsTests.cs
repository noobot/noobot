using Moq;
using SlackConnector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Noobot.Core;
using Shouldly;

namespace Noobot.Tests.Unit.Core.Slack
{
    public class ExtensionsTests
    {
        
        [Fact]
        public void FindByEmail_WithEmailSetExtension_ReturnsOnlyRecordsWithEmailSet()
        {
            //Arrange
            var slackUser = new SlackUser()
            {
                Id = "ABC",
                Email = "john.doe@microsoft.com"
            };

            var userCache = new ReadOnlyDictionary<string, SlackUser>(new Dictionary<string, SlackUser>
            {
                { slackUser.Id, slackUser }
            });


            //Act
            var result = userCache.WithEmailSet();

            //Assert
            result.First().Value.Email.ShouldNotBeNull();
        }

        [Fact]
        public void FindByEmail_WithoutEmailSet_DoesNotReturnUser()
        {
            //Arrange
            var slackUser = new SlackUser()
            {
                Id = "ABC",
            };

            var userCache = new ReadOnlyDictionary<string, SlackUser>(new Dictionary<string, SlackUser>
            {
                { slackUser.Id, slackUser }
            });


            //Act
            var result = userCache.WithEmailSet();

            //Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void GetUserIdForEmail_WithEmailSet_ReturnsId()
        {
            //Arrange
            var slackUser = new SlackUser()
            {
                Id = "ABC",
                Email = "john.doe@microsoft.com"
            };

            var userCache = new ReadOnlyDictionary<string, SlackUser>(new Dictionary<string, SlackUser>
            {
                { slackUser.Id, slackUser }
            });


            //Act
            var result = userCache.FindByEmail("john.doe@microsoft.com");

            //Assert
            result.Id.ShouldBe("ABC");
        }

        [Fact]
        public void GetUserIdForEmail_WithoutEmailSet_ReturnsNull()
        {
            //Arrange
            var slackUser = new SlackUser()
            {
                Id = "ABC"
            };

            var userCache = new ReadOnlyDictionary<string, SlackUser>(new Dictionary<string, SlackUser>
            {
                { slackUser.Id, slackUser }
            });


            //Act
            var result = userCache.FindByEmail("john.doe@microsoft.com");

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetUserIdForEmail_MultipleUsersWithEmailSet_ReturnsCorrectUser()
        {
            //Arrange
            var slackUser1 = new SlackUser()
            {
                Id = "ABC",
                Email = "john.doe@microsoft.com"
            };
            var slackUser2 = new SlackUser()
            {
                Id = "DEF",
                Email = "john.doe@gmail.com"
            };
            var slackUser3 = new SlackUser()
            {
                Id = "QWE",
                Email = "john.doe@yahoo.com"
            };

            var userCache = new ReadOnlyDictionary<string, SlackUser>(new Dictionary<string, SlackUser>
            {
                { slackUser1.Id, slackUser1 },
                { slackUser2.Id, slackUser2 },
                { slackUser3.Id, slackUser3 },
            });


            //Act
            var result = userCache.FindByEmail("john.doe@microsoft.com");

            //Assert
            result.Id.ShouldBe("ABC");
        }
    }
}
