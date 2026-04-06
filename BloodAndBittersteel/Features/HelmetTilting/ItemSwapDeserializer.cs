namespace BloodAndBittersteel.Features.HelmetTilting
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    public record ItemSwap
    {
        [XmlElement("ItemId")]
        public List<string> ItemIds { get; init; } = new();
    }
    public static class ItemSwapDeserializer
    {
        public static List<ItemSwap> LoadFromFile(string path)
        {
            var serializer = new XmlSerializer(
                typeof(List<ItemSwap>),
                new XmlRootAttribute("ItemSwaps")
            );

            using var stream = new FileStream(path, FileMode.Open);
            return (List<ItemSwap>)serializer.Deserialize(stream)!;
        }
    }
}
