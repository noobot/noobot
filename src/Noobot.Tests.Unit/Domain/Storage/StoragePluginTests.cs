using Noobot.Domain.Plugins.StandardPlugins;
using NUnit.Framework;
using SpecsFor.ShouldExtensions;

namespace Noobot.Tests.Unit.Domain.Storage
{
    [TestFixture]
    public class StoragePluginTests
    {
        [Test]
        public void should_write_and_read_from_file()
        {
            // given
            var storageHelper = new StoragePlugin();
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