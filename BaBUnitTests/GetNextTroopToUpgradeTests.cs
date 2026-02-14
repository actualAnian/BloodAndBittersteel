using BaBUnitTests.TestUtilities;
using HarmonyLib;
using LanceSystem.Deserialization;
using LanceSystem.Utils;
using System.Reflection;
using System.Runtime.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class GetNextTroopToUpgradeTests
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
        static PropertyInfo upgradeTargetsProperty = typeof(CharacterObject).GetProperty("UpgradeTargets", BindingFlags.Instance | BindingFlags.Public);

        static PropertyInfo defaultFormationProperty = typeof(CharacterObject).GetProperty("DefaultFormationClass", BindingFlags.Instance | BindingFlags.Public);

        [TestMethod]
        public void GetNextTroopToUpgrade_NoUpgradeTargets_ReturnsNull()
        {
            var src = new CharacterObject
            {
                Level = 1
            };
            upgradeTargetsProperty.SetValue(src, new CharacterObject[0]);
            var roster = TroopRoster.CreateDummyTroopRoster();
            roster.AddToCounts(src, 5);
            var caps = new List<float>(); // no caps needed for this test
            var result = LanceModelUtils.GetNextTroopToUpgrade(caps, roster, LanceTroopCategory.Infantry);

            Assert.IsNull(result, "When all characters have empty UpgradeTargets the result should be null.");
        }

        [TestMethod]
        public void GetNextTroopToUpgrade_SingleValidCandidate_ReturnsThatCharacter()
        {
            var src = new CharacterObject
            {
                Level = 1
            };

            var upTarget = new CharacterObject
            {
                Level = 2,
            };
            defaultFormationProperty.SetValue(upTarget, FormationClass.Infantry);
            upgradeTargetsProperty.SetValue(src, new[] { upTarget });

            var roster = TroopRoster.CreateDummyTroopRoster();
            roster.AddToCounts(src, 3);
            var caps = new List<float> { 1.0f, 1.0f, 1.0f };
            var result = LanceModelUtils.GetNextTroopToUpgrade(caps, roster, LanceTroopCategory.Infantry);

            Assert.IsNotNull(result, "There is a valid candidate, result should not be null.");
            Assert.AreSame(src, result, "With a single valid candidate it should be selected.");
        }

        [TestMethod]
        public void GetNextTroopToUpgrade_CapExceeded_NoValidUpgrades_ReturnsNull()
        {
            var src = new CharacterObject
            {
                Level = 1
            };
            var upTarget = new CharacterObject
            {
                Level = 2
            };

            defaultFormationProperty.SetValue(upTarget, FormationClass.Infantry);
            upgradeTargetsProperty.SetValue(src, new[] { upTarget });

            var roster = TroopRoster.CreateDummyTroopRoster();
            roster.AddToCounts(src, 5);
            var caps = new List<float> { 1.0f, 1.0f, 0.0f };
            var result = LanceModelUtils.GetNextTroopToUpgrade(caps, roster, LanceTroopCategory.Infantry);
            Assert.IsNull(result, "When the target tier cap is reached/exceeded, no upgrade should be allowed and result should be null.");
        }
    }
}