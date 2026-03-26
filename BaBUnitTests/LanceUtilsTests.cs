using LanceSystem.LanceDataClasses;
using LanceSystem.Logger;
using LanceSystem.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class LanceUtilsTests
    {
        [TestInitialize]
        public void Setup()
        {
            LanceUtils.UtilsRandom = new Random(12345);
            _testLogger = new TestLogger();
            LanceLogger.Logger = _testLogger;
        }
        private static TestLogger _testLogger = new();
        private static int SumLanceCounts(List<LanceData> lances, CharacterObject character)
        {
            int sum = 0;
            for (int i = 0; i < lances.Count; i++)
                sum += lances[i].LanceRoster.GetTroopCount(character);
            return sum;
        }
       
        [TestMethod]
        public void NormalizeLanceTroopsToParty_DoesNotCreateNegativeCounts()
        {
            var troop = new CharacterObject();
            var partyRoster = TroopRoster.CreateDummyTroopRoster();
            partyRoster.AddToCounts(troop, 0);

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(troop, 10);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(troop, 10);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1, "", "", ""),
                new NotableLanceData(r2, "", "", "")
            };
            LanceUtils.NormalizeLanceTroopsToParty(partyRoster, lances);

            var finalTotal = SumLanceCounts(lances, troop);
            Assert.AreEqual(0, finalTotal);
        }
        [TestMethod]
        public void NormalizeLanceTroopsToParty_NoExcess_NoChange()
        {
            var troop = new CharacterObject();

            var partyRoster = TroopRoster.CreateDummyTroopRoster();
            partyRoster.AddToCounts(troop, 5);

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(troop, 2);

            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(troop, 3);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1,"","",""),
                new NotableLanceData(r2,"","","")
            };

            var originalTotal = SumLanceCounts(lances, troop);

            LanceUtils.NormalizeLanceTroopsToParty(partyRoster, lances);

            var finalTotal = SumLanceCounts(lances, troop);

            Assert.AreEqual(originalTotal, finalTotal);
        }
        [TestMethod]
        public void NormalizeLanceTroopsToParty_TroopOnlyInLances_Removed()
        {
            var troop = new CharacterObject();

            var partyRoster = TroopRoster.CreateDummyTroopRoster();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(troop, 6);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1,"","","")
            };

            LanceUtils.NormalizeLanceTroopsToParty(partyRoster, lances);

            Assert.AreEqual(0, SumLanceCounts(lances, troop));
        }
        [TestMethod]
        public void NormalizeLanceTroopsToParty_ExcessRemoved_CorrectAmount()
        {
            var troop = new CharacterObject();

            var partyRoster = TroopRoster.CreateDummyTroopRoster();
            partyRoster.AddToCounts(troop, 3);

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(troop, 7);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1,"","","")
            };

            LanceUtils.NormalizeLanceTroopsToParty(partyRoster, lances);

            Assert.AreEqual(3, SumLanceCounts(lances, troop));
        }
        [TestMethod]
        public void NormalizeLanceTroopsToParty_MultipleTroops_HandledIndependently()
        {
            var t1 = new CharacterObject();
            var t2 = new CharacterObject();

            var partyRoster = TroopRoster.CreateDummyTroopRoster();
            partyRoster.AddToCounts(t1, 2);
            partyRoster.AddToCounts(t2, 1);

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(t1, 5);
            r1.AddToCounts(t2, 1);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1,"","","")
            };

            LanceUtils.NormalizeLanceTroopsToParty(partyRoster, lances);

            Assert.AreEqual(2, SumLanceCounts(lances, t1));
            Assert.AreEqual(1, SumLanceCounts(lances, t2));
        }
        [TestMethod]
        public void RemoveTroopsRandomlyFromLances_DoesNotCreateNegativeCounts()
        {
            var mockObj = new CharacterObject();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(mockObj, 1);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(mockObj, 1);

            var lances = new List<LanceData> { new NotableLanceData(r1, string.Empty, string.Empty, string.Empty), new NotableLanceData(r2, string.Empty, string.Empty, string.Empty) };

            var originalCounts = new List<int> { r1.GetTroopCount(mockObj), r2.GetTroopCount(mockObj) };
            int toRemove = 3;
            LanceUtils.RemoveTroopsRandomlyFromLances(mockObj, toRemove, lances);

            var newCounts = new List<int> { r1.GetTroopCount(mockObj), r2.GetTroopCount(mockObj) };
            for (int i = 0; i < newCounts.Count; i++)
            {
                Assert.IsGreaterThanOrEqualTo(0, newCounts[i], "Lance roster count became negative");
                Assert.IsLessThanOrEqualTo(originalCounts[i], newCounts[i], "Lance roster count increased unexpectedly");
            }

            // Total should equal member count (or unchanged if no excess)
            var finalTotal = SumLanceCounts(lances, mockObj);
            Assert.AreEqual(0, finalTotal);
        }
        [TestMethod]
        public void RemoveTroopsRandomlyFromLances_Excess_RemovesDownToMemberCount()
        {
            var mockObj = new CharacterObject();
            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(mockObj, 3);
            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(mockObj, 5);

            var lances = new List<LanceData> { new NotableLanceData(r1, string.Empty, string.Empty, string.Empty), new NotableLanceData(r2, string.Empty, string.Empty, string.Empty) };

            int toRemove = 3;
            LanceUtils.RemoveTroopsRandomlyFromLances(mockObj, toRemove, lances);

            var finalTotal = SumLanceCounts(lances, mockObj);
            // After removal total in lances should equal member count
            Assert.AreEqual(5, finalTotal);
        }

        // add troops randomly
        [TestMethod]
        public void UpgradeTroopsRandomlyInLances_BasicUpgrade_WorksCorrectly()
        {
            var from = new CharacterObject();
            var to = new CharacterObject();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(from, 5);

            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(from, 5);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1, "", "", ""),
                new NotableLanceData(r2, "", "", "")
            };

            int toAdd = 6;

            LanceUtils.UpgradeTroopsRandomlyInLances(from, to, toAdd, lances);

            int remainingFrom = SumLanceCounts(lances, from);
            int addedTo = SumLanceCounts(lances, to);

            Assert.AreEqual(4, remainingFrom);
            Assert.AreEqual(6, addedTo);
        }
        [TestMethod]
        public void UpgradeTroopsRandomlyInLances_ToAddZero_NoChange()
        {
            var from = new CharacterObject();
            var to = new CharacterObject();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(from, 3);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1, "", "", "")
            };

            int originalFrom = SumLanceCounts(lances, from);

            LanceUtils.UpgradeTroopsRandomlyInLances(from, to, 0, lances);

            Assert.AreEqual(originalFrom, SumLanceCounts(lances, from));
            Assert.AreEqual(0, SumLanceCounts(lances, to));
        }
        [TestMethod]
        public void UpgradeTroopsRandomlyInLances_NoFromTroops_NoChange()
        {
            var from = new CharacterObject();
            var to = new CharacterObject();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            var r2 = TroopRoster.CreateDummyTroopRoster();

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1, "", "", ""),
                new NotableLanceData(r2, "", "", "")
            };

            LanceUtils.UpgradeTroopsRandomlyInLances(from, to, 5, lances);

            Assert.AreEqual(0, SumLanceCounts(lances, from));
            Assert.AreEqual(0, SumLanceCounts(lances, to));
        }
        [TestMethod]
        public void UpgradeTroopsRandomlyInLances_RequestMoreThanAvailable_Clamped()
        {
            var from = new CharacterObject();
            var to = new CharacterObject();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(from, 2);

            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(from, 1);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1, "", "", ""),
                new NotableLanceData(r2, "", "", "")
            };
            LanceUtils.UpgradeTroopsRandomlyInLances(from, to, 10, lances);

            Assert.AreEqual(0, SumLanceCounts(lances, from));
            Assert.AreEqual(3, SumLanceCounts(lances, to));
        }
        [TestMethod]
        public void UpgradeTroopsRandomlyInLances_SingleLance_AllUpgradedFromThatLance()
        {
            var from = new CharacterObject();
            var to = new CharacterObject();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(from, 5);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1, "", "", "")
            };

            LanceUtils.UpgradeTroopsRandomlyInLances(from, to, 3, lances);

            Assert.AreEqual(2, r1.GetTroopCount(from));
            Assert.AreEqual(3, r1.GetTroopCount(to));
        }
        [TestMethod]
        public void UpgradeTroopsRandomlyInLances_ManyLancesUpgraded()
        {
            var from = new CharacterObject();
            var to = new CharacterObject();

            var r1 = TroopRoster.CreateDummyTroopRoster();
            r1.AddToCounts(from, 3);

            var r2 = TroopRoster.CreateDummyTroopRoster();
            r2.AddToCounts(from, 3);

            var lances = new List<LanceData>
            {
                new NotableLanceData(r1, "", "", ""),
                new NotableLanceData(r2, "", "", "")
            };
            LanceUtils.UpgradeTroopsRandomlyInLances(from, to, 5, lances);
            
            Assert.AreEqual(2, r1.GetTroopCount(to));
            Assert.AreEqual(3, r2.GetTroopCount(to));
        }
        [TestMethod]
        public void RemoveLowestTierTroops_RemovesLowestTierFirst()
        {
            var t1 = new CharacterObject { Level = 1};
            var t2 = new CharacterObject { Level = 2};

            var roster = TroopRoster.CreateDummyTroopRoster();
            roster.AddToCounts(t1, 5);
            roster.AddToCounts(t2, 5);

            LanceUtils.RemoveLowestTierTroops(roster, 3);

            Assert.AreEqual(2, roster.GetTroopCount(t1)); // removed 3
            Assert.AreEqual(5, roster.GetTroopCount(t2)); // untouched
        }
        [TestMethod]
        public void RemoveLowestTierTroops_SpillsIntoNextTier()
        {
            var t1 = new CharacterObject { Level = 1};
            var t2 = new CharacterObject { Level = 2};

            var roster = TroopRoster.CreateDummyTroopRoster();
            roster.AddToCounts(t1, 2);
            roster.AddToCounts(t2, 5);

            LanceUtils.RemoveLowestTierTroops(roster, 4);

            Assert.AreEqual(0, roster.GetTroopCount(t1)); // removed all 2
            Assert.AreEqual(3, roster.GetTroopCount(t2)); // removed 2 more
        }
        [TestMethod]
        public void RemoveLowestTierTroops_RemoveMoreThanAvailable_RemovesAll()
        {
            var t1 = new CharacterObject { Level = 1};

            var roster = TroopRoster.CreateDummyTroopRoster();
            roster.AddToCounts(t1, 3);

            LanceUtils.RemoveLowestTierTroops(roster, 10);
            Assert.AreEqual(0, roster.GetTroopCount(t1));
        }
        [TestMethod]
        public void RemoveLowestTierTroops_ZeroAmount_NoChange()
        {
            var t1 = new CharacterObject { Level = 1};

            var roster = TroopRoster.CreateDummyTroopRoster();
            roster.AddToCounts(t1, 5);

            LanceUtils.RemoveLowestTierTroops(roster, 0);
            Assert.AreEqual(5, roster.GetTroopCount(t1));
        }
    }
}
