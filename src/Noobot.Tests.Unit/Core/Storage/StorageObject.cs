using FlatFile.Delimited.Attributes;

namespace Noobot.Tests.Unit.Core.Storage
{
    [DelimitedFile(Delimiter = ";", Quotes = "\"")]
    public class StorageObject
    {
        [DelimitedField(1)]
        public int Id { get; set; }

        [DelimitedField(2)]
        public string Description { get; set; }

        [DelimitedField(3, NullValue = "=Null")]
        public int? NullableInt { get; set; }
    }
}