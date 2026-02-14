using LanceSystem.Deserialization;
using LanceSystem.Utils;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class BaBLanceModelTests
    {
        public TestContext TestContext { get; set; } // Add this property

        [TestMethod]
        public void GetTroopTypeDistribution_AggregatesCountsByType()
        {
            var pairs = new List<(LanceTroopCategory type, int number)>
            {
                (LanceTroopCategory.Infantry, 3),
                (LanceTroopCategory.Ranged, 2),
                (LanceTroopCategory.Cavalry, 5),
                (LanceTroopCategory.Infantry, 4),
                (LanceTroopCategory.HorseArcher, 1)
            };

            var res = LanceModelUtils.GetTroopTypeDistributionFromPairs(pairs);

            Assert.AreEqual(7, res[LanceTroopCategory.Infantry]);
            Assert.AreEqual(2, res[LanceTroopCategory.Ranged]);
            Assert.AreEqual(5, res[LanceTroopCategory.Cavalry]);
            Assert.AreEqual(1, res[LanceTroopCategory.HorseArcher]);
        }

        [TestMethod]
        public void ChooseTroopTypeToGet_PrefersUnderrepresented_WhenWithinLikelihood()
        {
            var current = new Dictionary<LanceTroopCategory, int>
            {
                { LanceTroopCategory.Infantry, 9 },
                { LanceTroopCategory.Ranged, 0 },
                { LanceTroopCategory.Cavalry, 1 },
                { LanceTroopCategory.HorseArcher, 0 }
            };

            var template = new LanceTroopsTemplate(
            [
                new TroopData(LanceTroopCategory.Infantry, 0.5, ""),
                new TroopData(LanceTroopCategory.Ranged, 0.25, ""),
                new TroopData(LanceTroopCategory.Cavalry, 0.2, ""),
                new TroopData(LanceTroopCategory.HorseArcher, 0.05, "")
            ]);

            var chosen = LanceModelUtils.DetermineTroopTypeToAdd(current, template);
            // Adding one ranged (0 -> 1) to total 10 => share 0.1 <= 0.25 -> acceptable; should pick the most underrepresented among acceptable
            Assert.AreEqual(LanceTroopCategory.Ranged, chosen);
        }

        [TestMethod]
        public void ChooseTroopTypeToGet_IfAllIncreaseAboveLikelihood_PicksHighestLikelihood()
        {
            var current = new Dictionary<LanceTroopCategory, int>
            {
                { LanceTroopCategory.Infantry, 50 },
                { LanceTroopCategory.Ranged, 49 },
                { LanceTroopCategory.Cavalry, 49 },
                { LanceTroopCategory.HorseArcher, 49 }
            };

            var template = new LanceTroopsTemplate(
            [
                new TroopData(LanceTroopCategory.Infantry, 0.01, "m"),
                new TroopData(LanceTroopCategory.Ranged, 0.01, "r"),
                new TroopData(LanceTroopCategory.Cavalry, 0.01, "c"),
                new TroopData(LanceTroopCategory.HorseArcher, 0.97, "h")
            ]);

            var chosen = LanceModelUtils.DetermineTroopTypeToAdd(current, template);
            Assert.AreEqual(LanceTroopCategory.HorseArcher, chosen);
        }

        [TestMethod]
        public void ChooseTroopTypeToGet_HandlesEmptyCurrentCounts()
        {
            var current = new Dictionary<LanceTroopCategory, int>();

            var template = new LanceTroopsTemplate(
            [
                new TroopData(LanceTroopCategory.Infantry, 0.4, "m"),
                new TroopData(LanceTroopCategory.Ranged, 0.3, "r"),
                new TroopData(LanceTroopCategory.Cavalry, 0.2, "c"),
                new TroopData(LanceTroopCategory.HorseArcher, 0.1, "h")
            ]);

            var chosen = LanceModelUtils.DetermineTroopTypeToAdd(current, template);
            // With empty current, total = 0; adding one yields share 1.0 for whichever chosen; none will be <= likelihood, so pick highest likelihood
            Assert.AreEqual(LanceTroopCategory.Infantry, chosen);
        }
        [TestMethod]
        public void ChooseTroopTypeToGet_NotFull()
        {
            var current = new Dictionary<LanceTroopCategory, int>();

            var template = new LanceTroopsTemplate(
            [
                new TroopData(LanceTroopCategory.Infantry, 0.6, "m"),
                new TroopData(LanceTroopCategory.Cavalry, 0.4, "c"),
            ]);

            var chosen = LanceModelUtils.DetermineTroopTypeToAdd(current, template);
            Assert.AreEqual(LanceTroopCategory.Infantry, chosen);
        }

        [TestMethod]
        public void ClampTroopQuality_NoChangeWhenSumBelowOne()
        {
            var quality = new List<float> { 0.2f, 0.3f, 0.1f };
            var original = new List<float>(quality);

            LanceModelUtils.ClampTroopQuality(quality);

            CollectionAssert.AreEqual(original, quality);
            Assert.AreEqual(0.6f, quality.Sum(), 1e-6);
        }

        [TestMethod]
        public void ClampTroopQuality_TrimsFromStartToEnforceSumOne()
        {
            var quality = new List<float> { 0.5f, 0.6f, 0.4f };
            LanceModelUtils.ClampTroopQuality(quality);

            // Expected result after algorithm: last-first accumulation clamps first element to 0, leaving [0,0.6,0.4]
            Assert.AreEqual(0.0f, quality[0], 1e-6);
            Assert.AreEqual(0.6f, quality[1], 1e-6);
            Assert.AreEqual(0.4f, quality[2], 1e-6);
            Assert.AreEqual(1.0f, quality.Sum(), 1e-6);
        }
    }
}
