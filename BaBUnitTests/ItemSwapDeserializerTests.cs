using BloodAndBittersteel.Features.HelmetTilting;
using System.IO;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class ItemSwapDeserializerTests
    {
        private static string CreateTempFile(string content)
        {
            var path = Path.GetTempFileName();
            File.WriteAllText(path, content);
            return path;
        }

        private static List<ItemSwap> Load(string xml)
        {
            var path = CreateTempFile(xml);
            try
            {
                return ItemSwapDeserializer.LoadFromFile(path);
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
                <ItemSwaps>
                  <ItemSwap>
                    <ItemId>sword</ItemId>
                  </ItemSwap>
                  <ItemSwap>
                    <ItemId>shield</ItemId>
                  </ItemSwap>
                </ItemSwaps>
                """);

            Assert.HasCount(2, swaps);
        }

        [TestMethod]
        public void LoadFromFile_PreservesItemIdOrder()
        {
            var swaps = Load(
                """
                <ItemSwaps>
                  <ItemSwap>
                    <ItemId>first</ItemId>
                    <ItemId>second</ItemId>
                    <ItemId>third</ItemId>
                  </ItemSwap>
                </ItemSwaps>
                """);

            CollectionAssert.AreEqual(new[] { "first", "second", "third" }, swaps[0].ItemIds);
        }

        [TestMethod]
        public void LoadFromFile_SingleSwap_SingleItemId()
        {
            var swaps = Load(
                """
                <ItemSwaps>
                  <ItemSwap>
                    <ItemId>only</ItemId>
                  </ItemSwap>
                </ItemSwaps>
                """);

            Assert.HasCount(1, swaps);
            CollectionAssert.AreEqual(new[] { "only" }, swaps[0].ItemIds);
        }

        [TestMethod]
        public void LoadFromFile_EmptyRoot_ReturnsEmptyList()
        {
            var swaps = Load("<ItemSwaps />");

            Assert.IsEmpty(swaps);
        }

        [TestMethod]
        public void LoadFromFile_MissingFile_Throws()
        {
            var missingPath = Path.Combine(Path.GetTempPath(), "does_not_exist_item_swap.xml");

            Assert.ThrowsExactly<FileNotFoundException>(() => ItemSwapDeserializer.LoadFromFile(missingPath));
        }
    }
}
