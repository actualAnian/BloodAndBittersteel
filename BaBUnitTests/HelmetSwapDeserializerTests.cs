using BloodAndBittersteel.Features.HelmetTilting;
using System.IO;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class HelmetSwapDeserializerTests
    {
        private static string CreateTempFile(string content)
        {
            var path = Path.GetTempFileName();
            File.WriteAllText(path, content);
            return path;
        }

        private static List<HelmetSwap> Load(string xml)
        {
            var path = CreateTempFile(xml);
            try
            {
                return HelmetSwapDeserializer.LoadFromFile(path);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void LoadFromFile_ParsesMultipleSwaps()
        {
            var swaps = Load(
                """
                <HelmetSwaps>
                  <HelmetSwap>
                    <VisorOpenedItemId>helmet_open</VisorOpenedItemId>
                    <VisorClosedItemId>helmet_closed</VisorClosedItemId>
                  </HelmetSwap>
                  <HelmetSwap>
                    <VisorOpenedItemId>other_open</VisorOpenedItemId>
                    <VisorClosedItemId>other_closed</VisorClosedItemId>
                  </HelmetSwap>
                </HelmetSwaps>
                """);

            Assert.HasCount(2, swaps);
        }

        [TestMethod]
        public void LoadFromFile_PreservesItemIds()
        {
            var swaps = Load(
                """
                <HelmetSwaps>
                  <HelmetSwap>
                    <VisorOpenedItemId>opened_helmet</VisorOpenedItemId>
                    <VisorClosedItemId>closed_helmet</VisorClosedItemId>
                  </HelmetSwap>
                </HelmetSwaps>
                """);

            Assert.AreEqual("opened_helmet", swaps[0].VisorOpenedItemId);
            Assert.AreEqual("closed_helmet", swaps[0].VisorClosedItemId);
        }

        [TestMethod]
        public void LoadFromFile_SingleSwap()
        {
            var swaps = Load(
                """
                <HelmetSwaps>
                  <HelmetSwap>
                    <VisorOpenedItemId>only_open</VisorOpenedItemId>
                    <VisorClosedItemId>only_closed</VisorClosedItemId>
                  </HelmetSwap>
                </HelmetSwaps>
                """);

            Assert.HasCount(1, swaps);
            Assert.AreEqual("only_open", swaps[0].VisorOpenedItemId);
            Assert.AreEqual("only_closed", swaps[0].VisorClosedItemId);
        }

        [TestMethod]
        public void LoadFromFile_EmptyRoot_ReturnsEmptyList()
        {
            var swaps = Load("<HelmetSwaps />");

            Assert.IsEmpty(swaps);
        }

        [TestMethod]
        public void LoadFromFile_MissingFile_Throws()
        {
            var missingPath = Path.Combine(Path.GetTempPath(), "does_not_exist_helmet_swap.xml");

            Assert.ThrowsExactly<FileNotFoundException>(() => HelmetSwapDeserializer.LoadFromFile(missingPath));
        }
    }
}
