namespace BloodAndBittersteel.Features.HelmetTilting
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public record HelmetSwap
    {
        [XmlElement("VisorOpenedItemId")]
        public string VisorOpenedItemId { get; init; } = string.Empty;

        [XmlElement("VisorClosedItemId")]
        public string VisorClosedItemId { get; init; } = string.Empty;
    }

    public static class HelmetSwapDeserializer
    {
        public static List<HelmetSwap> LoadFromFile(string path)
        {
            var serializer = new XmlSerializer(
                typeof(List<HelmetSwap>),
                new XmlRootAttribute("HelmetSwaps")
            );

            using var stream = new FileStream(path, FileMode.Open);
            return (List<HelmetSwap>)serializer.Deserialize(stream)!;
        }
    }
}
