using LanceSystem.LanceDataClasses;
using LanceSystem.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class LanceUtilsTests
    {
        private static int SumLanceCounts(List<LanceData> lances, CharacterObject character)
        {
            int sum = 0;
            for (int i = 0; i < lances.Count; i++)
                sum += lances[i].LanceRoster.GetTroopCount(character);
            return sum;
        }

        [TestMethod]
        public void CalculateNumberOfTroopsToRemove_NoLances_ReturnsNegativeMemberCount()
        {
            var mockObj = new CharacterObject();
            var el = new TroopRosterElement(mockObj)
            {
                Number = 5
            };
            var lances = new List<LanceData>();

            int result = LanceUtils.CalculateNumberOfTroopsToRemove(el, lances);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateNumberOfTroopsToRemove_LancesWithMoreTroops_ReturnsPositiveDifference()
        {
            var mockObj = new CharacterObject();
            var el = new TroopRosterElement(mockObj)
            {
                Number = 5
            };

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(mockObj, 3);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(mockObj, 5);

            var lances = new List<LanceData> { new NotableLanceData(r1, string.Empty, string.Empty, string.Empty), new NotableLanceData(r2, string.Empty, string.Empty, string.Empty) };

            int result = LanceUtils.CalculateNumberOfTroopsToRemove(el, lances);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void CalculateNumberOfTroopsToRemove_LancesEqualToMembers_ReturnsZero()
        {
            var mockObj = new CharacterObject();
            var el = new TroopRosterElement(mockObj)
            {
                Number = 5
            };

            var r = TroopRoster.CreateDummyTroopRoster();
            r.AddToCounts(mockObj, 5);

            var lances = new List<LanceData> { new NotableLanceData(r, string.Empty, string.Empty, string.Empty) };

            int result = LanceUtils.CalculateNumberOfTroopsToRemove(el, lances);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateNumberOfTroopsToRemove_MultipleLances_VariedCounts()
        {
            var mockObj = new CharacterObject();
            var el = new TroopRosterElement(mockObj)
            {
                Number = 10
            };
            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(mockObj, 2);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(mockObj, 4);
            var r3 = TroopRoster.CreateDummyTroopRoster();
            r3.AddToCounts(mockObj, 6);

            var lances = new List<LanceData> { new NotableLanceData(r1, string.Empty, string.Empty, string.Empty), new NotableLanceData(r2, string.Empty, string.Empty, string.Empty), new NotableLanceData(r3, string.Empty, string.Empty, string.Empty) };

            int result = LanceUtils.CalculateNumberOfTroopsToRemove(el, lances);

            // Total in lances = 2+4+6 = 12; members = 10 -> 2
            Assert.AreEqual(2, result);
        }

        // New tests for RemoveTroopsRandomlyFromLances

        [TestMethod]
        public void RemoveTroopsRandomlyFromLances_NoExcess_NoChange()
        {
            var mockObj = new CharacterObject();
            var el = new TroopRosterElement(mockObj)
            {
                Number = 10 // members greater than total in lances
            };

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(mockObj, 3);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(mockObj, 4);

            var lances = new List<LanceData> { new NotableLanceData(r1, string.Empty, string.Empty, string.Empty), new NotableLanceData(r2, string.Empty, string.Empty, string.Empty) };

            var originalTotal = SumLanceCounts(lances, mockObj);
            int toRemove = LanceUtils.CalculateNumberOfTroopsToRemove(el, lances);

            // toRemove should be negative or zero, so nothing should change
            LanceUtils.RemoveTroopsRandomlyFromLances(el, toRemove, lances);

            var finalTotal = SumLanceCounts(lances, mockObj);
            Assert.AreEqual(originalTotal, finalTotal);
        }

        [TestMethod]
        public void RemoveTroopsRandomlyFromLances_Excess_RemovesDownToMemberCount()
        {
            var mockObj = new CharacterObject();
            var el = new TroopRosterElement(mockObj)
            {
                Number = 5
            };

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(mockObj, 3);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(mockObj, 5);

            var lances = new List<LanceData> { new NotableLanceData(r1, string.Empty, string.Empty, string.Empty), new NotableLanceData(r2, string.Empty, string.Empty, string.Empty) };

            SumLanceCounts(lances, mockObj);
            int toRemove = LanceUtils.CalculateNumberOfTroopsToRemove(el, lances);
            Assert.AreEqual(3, toRemove);

            LanceUtils.RemoveTroopsRandomlyFromLances(el, toRemove, lances);

            var finalTotal = SumLanceCounts(lances, mockObj);
            // After removal total in lances should equal member count
            Assert.AreEqual(el.Number, finalTotal);
        }

        [TestMethod]
        public void RemoveTroopsRandomlyFromLances_DoesNotCreateNegativeCounts()
        {
            var mockObj = new CharacterObject();
            var el = new TroopRosterElement(mockObj)
            {
                Number = 0
            };

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(mockObj, 10);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(mockObj, 10);

            var lances = new List<LanceData> { new NotableLanceData(r1, string.Empty, string.Empty, string.Empty), new NotableLanceData(r2, string.Empty, string.Empty, string.Empty) };

            var originalCounts = new List<int> { r1.GetTroopCount(mockObj), r2.GetTroopCount(mockObj) };
            int toRemove = LanceUtils.CalculateNumberOfTroopsToRemove(el, lances);

            LanceUtils.RemoveTroopsRandomlyFromLances(el, toRemove, lances);

            var newCounts = new List<int> { r1.GetTroopCount(mockObj), r2.GetTroopCount(mockObj) };
            for (int i = 0; i < newCounts.Count; i++)
            {
                Assert.IsGreaterThanOrEqualTo(0, newCounts[i], "Lance roster count became negative");
                Assert.IsLessThanOrEqualTo(originalCounts[i], newCounts[i], "Lance roster count increased unexpectedly");
            }

            // Total should equal member count (or unchanged if no excess)
            var finalTotal = SumLanceCounts(lances, mockObj);
            Assert.AreEqual(el.Number, finalTotal);
        }
    }
}
