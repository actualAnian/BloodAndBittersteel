using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BloodAndBittersteel.Features.LanceSystem;
using BloodAndBittersteel.Features.LanceSystem.Deserialization;
using System.Collections.Generic;
using System.Linq;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class BaBLanceModelTests
    {
        public TestContext TestContext { get; set; } // Add this property

        [TestMethod]
        public void GetTroopTypeDistribution_AggregatesCountsByType()
        {
            var pairs = new List<(LanceTroopType type, int number)>
            {
                (LanceTroopType.Infantry, 3),
                (LanceTroopType.Ranged, 2),
                (LanceTroopType.Cavalry, 5),
                (LanceTroopType.Infantry, 4),
                (LanceTroopType.HorseArcher, 1)
            };

            var res = LanceModelUtils.GetTroopTypeDistributionFromPairs(pairs);

            Assert.AreEqual(7, res[LanceTroopType.Infantry]);
            Assert.AreEqual(2, res[LanceTroopType.Ranged]);
            Assert.AreEqual(5, res[LanceTroopType.Cavalry]);
            Assert.AreEqual(1, res[LanceTroopType.HorseArcher]);
        }

        [TestMethod]
        public void ChooseTroopTypeToGet_PrefersUnderrepresented_WhenWithinLikelihood()
        {
            var current = new Dictionary<LanceTroopType, int>
            {
                { LanceTroopType.Infantry, 9 },
                { LanceTroopType.Ranged, 0 },
                { LanceTroopType.Cavalry, 1 },
                { LanceTroopType.HorseArcher, 0 }
            };

            var template = new LanceTroopsTemplate(
                new TroopData(0.5, ""), // melee
                new TroopData(0.25, ""),
                new TroopData(0.2, ""),
                new TroopData(0.05, "")
            );

            var chosen = LanceModelUtils.DetermineTroopTypeToAdd(current, template);
            // Adding one ranged (0 -> 1) to total 10 => share 0.1 <= 0.25 -> acceptable; should pick the most underrepresented among acceptable
            Assert.AreEqual(LanceTroopType.Ranged, chosen);
        }

        [TestMethod]
        public void ChooseTroopTypeToGet_IfAllIncreaseAboveLikelihood_PicksHighestLikelihood()
        {
            var current = new Dictionary<LanceTroopType, int>
            {
                { LanceTroopType.Infantry, 50 },
                { LanceTroopType.Ranged, 49 },
                { LanceTroopType.Cavalry, 49 },
                { LanceTroopType.HorseArcher, 49 }
            };

            var template = new LanceTroopsTemplate(
                new TroopData(0.01, "m"),
                new TroopData(0.01, "r"),
                new TroopData(0.01, "c"),
                new TroopData(0.97, "h")
            );

            var chosen = LanceModelUtils.DetermineTroopTypeToAdd(current, template);
            Assert.AreEqual(LanceTroopType.HorseArcher, chosen);
        }

        [TestMethod]
        public void ChooseTroopTypeToGet_HandlesEmptyCurrentCounts()
        {
            var current = new Dictionary<LanceTroopType, int>();

            var template = new LanceTroopsTemplate(
                new TroopData(0.4, "m"),
                new TroopData(0.3, "r"),
                new TroopData(0.2, "c"),
                new TroopData(0.1, "h")
            );

            var chosen = LanceModelUtils.DetermineTroopTypeToAdd(current, template);
            // With empty current, total = 0; adding one yields share 1.0 for whichever chosen; none will be <= likelihood, so pick highest likelihood
            Assert.AreEqual(LanceTroopType.Infantry, chosen);
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
