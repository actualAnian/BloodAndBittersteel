using BaBUnitTests.TestUtilities;
using BloodAndBittersteel.Library.ModifiableValues;
using System.Reflection;
using System.Runtime.Serialization;
using TaleWorlds.CampaignSystem;

namespace BaBUnitTests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class TimedModifierNumberTests
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
            typeof(GameModels)
                .GetProperty("CharacterStatsModel", BindingFlags.Instance | BindingFlags.Public)
                .SetValue(cam.Models, new TestCharacterStatsModel());
            typeof(GameModels)
                .GetProperty("CampaignTimeModel", BindingFlags.Instance | BindingFlags.Public)
                .SetValue(cam.Models, new TestCampaignTimeModel());

            CampaignTime.Initialize();

            var mapTimeTracker = FormatterServices.GetUninitializedObject(typeof(Campaign).GetProperty("MapTimeTracker", BindingFlags.Instance | BindingFlags.NonPublic).PropertyType);
            typeof(Campaign)
                .GetProperty("MapTimeTracker", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(cam, mapTimeTracker);
        }

        private static TimedModifier ActiveModifier(string id, float factor)
        {
            return new TimedModifier(id, factor, CampaignTime.Days(7));
        }

        private static TimedModifier ExpiredModifier(string id, float factor)
        {
            return new TimedModifier(id, factor, CampaignTime.Days(-1));
        }

        [TestMethod]
        public void AddModifier_AddsFactorToCurrentValue()
        {
            var number = new TimedModifierNumber();

            number.AddModifier(ActiveModifier("m1", 0.5f));

            Assert.AreEqual(0.5f, number.CurrentValue, 1e-6f);
        }

        [TestMethod]
        public void AddModifier_ReplacesExistingModifierWithSameId()
        {
            var number = new TimedModifierNumber();

            number.AddModifier(ActiveModifier("m1", 0.3f));
            number.AddModifier(ActiveModifier("m1", 0.7f));

            Assert.AreEqual(0.7f, number.CurrentValue, 1e-6f);
            Assert.HasCount(1, number._modifiers);
        }

        [TestMethod]
        public void AddModifier_DoesNotReplaceModifierWithDifferentId()
        {
            var number = new TimedModifierNumber();

            number.AddModifier(ActiveModifier("m1", 0.3f));
            number.AddModifier(ActiveModifier("m2", 0.6f));

            Assert.HasCount(2, number._modifiers);
            Assert.AreEqual(0.9f, number.CurrentValue, 1e-6f);
        }

        [TestMethod]
        public void RemoveModifier_RemovesById()
        {
            var number = new TimedModifierNumber();
            number.AddModifier(ActiveModifier("m1", 0.3f));
            number.AddModifier(ActiveModifier("m2", 0.6f));

            number.RemoveModifier("m1");

            Assert.AreEqual(0.6f, number.CurrentValue, 1e-6f);
        }

        [TestMethod]
        public void RemoveModifier_NonexistentId_NoThrow()
        {
            var number = new TimedModifierNumber();
            number.AddModifier(ActiveModifier("m1", 0.3f));

            number.RemoveModifier("does_not_exist");

            Assert.AreEqual(0.3f, number.CurrentValue, 1e-6f);
        }

        [TestMethod]
        public void CurrentValue_BaseValueOnly_NoModifiers()
        {
            var number = new TimedModifierNumber(5);

            Assert.AreEqual(5, number.CurrentValue, 1e-6f);
        }

        [TestMethod]
        public void CurrentValue_ExcludesExpiredModifiers()
        {
            var number = new TimedModifierNumber();
            number.AddModifier(ActiveModifier("active", 1.0f));
            number.AddModifier(ExpiredModifier("expired", 1.0f));

            Assert.AreEqual(1.0f, number.CurrentValue, 1e-6f);
        }

        [TestMethod]
        public void CurrentValue_SumsAllActiveModifiers()
        {
            var number = new TimedModifierNumber(10);
            number.AddModifier(ActiveModifier("m1", 1.0f));
            number.AddModifier(ActiveModifier("m2", 2.0f));

            Assert.AreEqual(13, number.CurrentValue, 1e-6f);
        }
    }
}
