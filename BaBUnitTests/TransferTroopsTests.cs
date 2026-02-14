using BaBUnitTests.TestUtilities;
using LanceSystem.Utils;
using System.Reflection;
using System.Runtime.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class TransferTroopsTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Campaign cam = (Campaign)FormatterServices.GetUninitializedObject(typeof(Campaign));
            var currentProp = typeof(Campaign)
                .GetProperty(
                    "Current",
                    BindingFlags.Static | BindingFlags.Public
                );
            currentProp.SetValue(null, cam);
            var modelsField = typeof(Campaign).GetField("_gameModels", BindingFlags.Instance | BindingFlags.NonPublic);
            GameModels models = (GameModels)FormatterServices.GetUninitializedObject(typeof(GameModels));
            modelsField.SetValue(cam, models);
            var prop = typeof(GameModels)
                .GetProperty(
                    "CharacterStatsModel",
                    BindingFlags.Instance | BindingFlags.Public
                );
            prop.SetValue(cam.Models, new TestCharacterStatsModel());
        }

        [TestMethod]
        public void Transfer_MovesHighestTierFirst_StandardFlow()
        {
            var cA = new CharacterObject() { };
            var cB = new CharacterObject() { };

            cA.Level = 3;
            cB.Level = 2;

            var from = TroopRoster.CreateDummyTroopRoster();
            from.AddToCounts(cA, 5); 
            from.AddToCounts(cB, 10);

            var to = TroopRoster.CreateDummyTroopRoster();

            // move 6 with large capacity: should move 5 of A and 1 of B
            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, 6, int.MaxValue);

            Assert.AreEqual(0, from.GetTroopCount(cA), "A should be fully moved");
            Assert.AreEqual(9, from.GetTroopCount(cB), "B should have one moved out");
            Assert.AreEqual(5, to.GetTroopCount(cA));
            Assert.AreEqual(1, to.GetTroopCount(cB));
        }

        [TestMethod]
        public void Transfer_TiesBrokenRandomly_OnlySameTierAffected()
        {
            var c1 = new CharacterObject() { };
            var c2 = new CharacterObject() { };
            var lower = new CharacterObject() { };

            c1.Level = 5;
            c2.Level = 5;
            lower.Level = 1;

            var from = TroopRoster.CreateDummyTroopRoster();
            from.AddToCounts(c1, 10);
            from.AddToCounts(c2, 10);
            from.AddToCounts(lower, 20);

            var to = TroopRoster.CreateDummyTroopRoster();

            // move 5 with large capacity: should only come from tier 5 characters (c1/c2), lower tier unchanged
            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, 5, int.MaxValue);

            // lower tier unchanged
            Assert.AreEqual(20, from.GetTroopCount(lower));
            Assert.AreEqual(0, to.GetTroopCount(lower));

            // total moved from tier 5 equals requested amount
            int movedFromTier5 = (10 - from.GetTroopCount(c1)) + (10 - from.GetTroopCount(c2));
            Assert.AreEqual(5, movedFromTier5);
            int movedToTier5 = to.GetTroopCount(c1) + to.GetTroopCount(c2);
            Assert.AreEqual(5, movedToTier5);
        }

        [TestMethod]
        public void Transfer_AmountLessThanOrEqualZero_NoChange()
        {
            var c = new CharacterObject
            {
                Level = 2
            };

            var from = TroopRoster.CreateDummyTroopRoster();
            from.AddToCounts(c, 5);
            var to = TroopRoster.CreateDummyTroopRoster();

            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, 0, 10);
            Assert.AreEqual(5, from.GetTroopCount(c));
            Assert.AreEqual(0, to.GetTroopCount(c));

            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, -3, 10);
            Assert.AreEqual(5, from.GetTroopCount(c));
            Assert.AreEqual(0, to.GetTroopCount(c));
        }

        [TestMethod]
        public void Transfer_AmountGreaterThanAvailable_MovesAll()
        {
            var a = new CharacterObject
            {
                Level = 2
            }; var b = new CharacterObject
            {
                Level = 1
            };
            var from = TroopRoster.CreateDummyTroopRoster();
            from.AddToCounts(a, 3);
            from.AddToCounts(b, 2);

            var to = TroopRoster.CreateDummyTroopRoster();

            // request to move more than total (10 vs 5)
            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, 10, int.MaxValue);

            Assert.AreEqual(0, from.GetTroopCount(a));
            Assert.AreEqual(0, from.GetTroopCount(b));
            Assert.AreEqual(3, to.GetTroopCount(a));
            Assert.AreEqual(2, to.GetTroopCount(b));
        }

        [TestMethod]
        public void Transfer_RespectsMaxCapacity_PartialFromHighestTier()
        {
            var cA = new CharacterObject() { };
            var cB = new CharacterObject() { };

            cA.Level = 3;
            cB.Level = 2;

            var from = TroopRoster.CreateDummyTroopRoster();
            from.AddToCounts(cA, 5); 
            from.AddToCounts(cB, 10);

            var to = TroopRoster.CreateDummyTroopRoster();

            // move 6 but capacity only 4 -> should move 4 from highest tier (cA)
            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, 6, 4);

            Assert.AreEqual(1, from.GetTroopCount(cA), "A should have 1 left (moved 4 of 5)");
            Assert.AreEqual(10, from.GetTroopCount(cB), "B should be untouched");
            Assert.AreEqual(4, to.GetTroopCount(cA));
            Assert.AreEqual(0, to.GetTroopCount(cB));
        }

        [TestMethod]
        public void Transfer_RespectsMaxCapacity_WhenToAlreadyHasTroops()
        {
            var cA = new CharacterObject() { };
            var cB = new CharacterObject() { };

            cA.Level = 3;
            cB.Level = 2;

            var from = TroopRoster.CreateDummyTroopRoster();
            from.AddToCounts(cA, 5);
            from.AddToCounts(cB, 10);

            var to = TroopRoster.CreateDummyTroopRoster();
            to.AddToCounts(cB, 3); // already has 3 troops total

            // max total allowed in 'to' is 5 -> only 2 more can be moved
            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, 10, 5);

            // Should have moved 2 from highest tier first (cA)
            Assert.AreEqual(3, from.GetTroopCount(cA), "A should have moved 2");
            Assert.AreEqual(10, from.GetTroopCount(cB), "B untouched");
            Assert.AreEqual(2, to.GetTroopCount(cA));
            Assert.AreEqual(3, to.GetTroopCount(cB));
        }

        [TestMethod]
        public void Transfer_NoMoveWhenMaxLessOrEqualCurrentToTotal()
        {
            var cA = new CharacterObject
            {
                Level = 3
            };

            var from = TroopRoster.CreateDummyTroopRoster();
            from.AddToCounts(cA, 5);

            var to = TroopRoster.CreateDummyTroopRoster();
            to.AddToCounts(cA, 5); // already at 5

            // max is 5 -> no capacity left
            LanceUtils.TransferTroopsBetweenTroopRosters(from, to, 3, 5);

            Assert.AreEqual(5, from.GetTroopCount(cA), "No troops should be moved");
            Assert.AreEqual(5, to.GetTroopCount(cA));
        }
    }
}