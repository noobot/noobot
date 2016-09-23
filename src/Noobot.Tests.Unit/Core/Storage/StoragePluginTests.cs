using Noobot.Core.Logging;
using Noobot.Toolbox.Plugins;
using NUnit.Framework;
using SpecsFor.ShouldExtensions;

namespace Noobot.Tests.Unit.Core.Storage
{
    [TestFixture]
    public class StoragePluginTests
    {
        [Test]
        public void should_write_and_read_from_file()
        {
            // given
            var storageHelper = new StoragePlugin(null);
            storageHelper.Start();

            var objects = new[]
            {
                new StorageObject {Id = 1, Description = "Doobee", NullableInt = 2123},
                new StorageObject {Id = 2, Description = "Minecraft", NullableInt = 543},
                new StorageObject {Id = 3, Description = "Crisps", NullableInt = 123},
            };

            // when
            storageHelper.SaveFile("example", objects);
            StorageObject[] result = storageHelper.ReadFile<StorageObject>("example");

            // then
            result.ShouldLookLike(objects);
        }
    }
}