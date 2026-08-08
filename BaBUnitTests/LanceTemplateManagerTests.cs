using LanceSystem.Deserialization;
using System.Reflection;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class LanceTemplateManagerTests
    {
        private static readonly LanceTroopsTemplate Troops = new([new(LanceTroopCategory.Infantry, 1.0, "looter")]);
        private static Lance LanceWith(string stringId, string? cultureId = null, string? clanId = null, LanceTemplateOriginType originType = LanceTemplateOriginType.All)
        {
            return new Lance(stringId, stringId, cultureId, clanId, originType, Troops);
        }

        private static LanceTemplateManager CreateManager(params Lance[] lances)
        {
            typeof(LanceTemplateManager).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, null);
            var manager = LanceTemplateManager.Instance;
            var dictionary = lances.ToDictionary(lance => lance.StringId);
            typeof(LanceTemplateManager).GetProperty("Lances")!.SetValue(manager, dictionary);
            return manager;
        }

        private static void AssertLanceIds(IEnumerable<Lance> result, params string[] expectedIds)
        {
            CollectionAssert.AreEquivalent(expectedIds, result.Select(lance => lance.StringId).ToArray());
        }

        [TestMethod]
        public void GetLances_ReturnsOnlyLancesForCulture_ExcludesOtherCultures()
        {
            var manager = CreateManager(
                LanceWith("empireLanceA", "empire"),
                LanceWith("empireLanceB", "empire"),
                LanceWith("vlandiaLance", "vlandia"),
                LanceWith("anyCultureLance"));

            var result = manager.GetLances("empire", null, LanceTemplateOriginType.All);

            AssertLanceIds(result, "empireLanceA", "empireLanceB", "anyCultureLance");
        }

        [TestMethod]
        public void GetLances_ReturnsOnlyLancesForClan_ExcludesOtherClans()
        {
            var manager = CreateManager(
                LanceWith("clanALanceA", null, "clanA"),
                LanceWith("clanALanceB", null, "clanA"),
                LanceWith("clanBLance", null, "clanB"),
                LanceWith("anyClanLance"));

            var result = manager.GetLances("empire", "clanA", LanceTemplateOriginType.All);

            AssertLanceIds(result, "clanALanceA", "clanALanceB", "anyClanLance");
        }

        [TestMethod]
        public void GetLances_ReturnsOnlyLancesForOrigin_ExcludesOtherOrigins()
        {
            var manager = CreateManager(
                LanceWith("mercenaryLance", originType: LanceTemplateOriginType.Mercenary),
                LanceWith("townLance", originType: LanceTemplateOriginType.Town),
                LanceWith("castleLance", originType: LanceTemplateOriginType.Castle),
                LanceWith("anyOriginLance", originType: LanceTemplateOriginType.All));

            var result = manager.GetLances("empire", null, LanceTemplateOriginType.Mercenary);

            AssertLanceIds(result, "mercenaryLance", "anyOriginLance");
        }

        [TestMethod]
        public void GetLances_MultipleLancesFulfillCondition_ReturnsAllMatching()
        {
            var manager = CreateManager(
                LanceWith("matchingOne", "empire", null, LanceTemplateOriginType.Town),
                LanceWith("matchingTwo", "empire", null, LanceTemplateOriginType.Town),
                LanceWith("matchingThree", "empire", null, LanceTemplateOriginType.All),
                LanceWith("wrongCulture", "vlandia", null, LanceTemplateOriginType.Town),
                LanceWith("wrongOrigin", "empire", null, LanceTemplateOriginType.Mercenary));

            var result = manager.GetLances("empire", null, LanceTemplateOriginType.Town);

            AssertLanceIds(result, "matchingOne", "matchingTwo", "matchingThree");
        }

        [TestMethod]
        public void GetLances_NoLancesMatch_ReturnsFallbackLance()
        {
            var manager = CreateManager(
                LanceWith("vlandiaTown", "vlandia", null, LanceTemplateOriginType.Town),
                LanceWith("vlandiaCastle", "vlandia", null, LanceTemplateOriginType.Castle),
                LanceWith("vlandiaMercenary", "vlandia", null, LanceTemplateOriginType.Mercenary));

            var result = manager.GetLances("empire", null, LanceTemplateOriginType.Town);

            AssertLanceIds(result, "fallback");
        }

        [TestMethod]
        public void GetLances_SettlementOriginLance_MatchesTownCastleAndVillage()
        {
            var manager = CreateManager(
                LanceWith("settlementLance", originType: LanceTemplateOriginType.Settlement),
                LanceWith("mercenaryLance", originType: LanceTemplateOriginType.Mercenary));

            AssertLanceIds(manager.GetLances("empire", null, LanceTemplateOriginType.Town), "settlementLance");
            AssertLanceIds(manager.GetLances("empire", null, LanceTemplateOriginType.Castle), "settlementLance");
            AssertLanceIds(manager.GetLances("empire", null, LanceTemplateOriginType.Village), "settlementLance");
        }

        [TestMethod]
        public void GetLances_SettlementOriginLance_DoesNotMatchMercenary()
        {
            var manager = CreateManager(
                LanceWith("settlementLance", originType: LanceTemplateOriginType.Settlement),
                LanceWith("mercenaryLance", originType: LanceTemplateOriginType.Mercenary));

            var result = manager.GetLances("empire", null, LanceTemplateOriginType.Mercenary);

            AssertLanceIds(result, "mercenaryLance");
        }

        [TestMethod]
        public void GetLances_AllOriginLance_MatchesAnyOrigin()
        {
            var manager = CreateManager(
                LanceWith("anyOriginLance", originType: LanceTemplateOriginType.All),
                LanceWith("townLance", originType: LanceTemplateOriginType.Town));

            AssertLanceIds(manager.GetLances("empire", null, LanceTemplateOriginType.Town), "anyOriginLance", "townLance");
            AssertLanceIds(manager.GetLances("empire", null, LanceTemplateOriginType.Mercenary), "anyOriginLance");
            AssertLanceIds(manager.GetLances("empire", null, LanceTemplateOriginType.Settlement), "anyOriginLance");
        }

        [TestMethod]
        public void GetLances_OriginQueryAll_OnlyMatchesAllOriginLances()
        {
            var manager = CreateManager(
                LanceWith("anyOriginLance", originType: LanceTemplateOriginType.All),
                LanceWith("townLance", originType: LanceTemplateOriginType.Town));

            var result = manager.GetLances("empire", null, LanceTemplateOriginType.All);

            AssertLanceIds(result, "anyOriginLance");
        }

        [TestMethod]
        public void GetLances_AllFiltersCombined_ReturnsOnlyExpectedLances()
        {
            var manager = CreateManager(
                LanceWith("matchAll", "empire", "clanA", LanceTemplateOriginType.Town),
                LanceWith("matchOriginAndCulture", "empire", "clanB", LanceTemplateOriginType.Town),
                LanceWith("wrongClan", "empire", "clanC", LanceTemplateOriginType.Town),
                LanceWith("wrongCulture", "vlandia", "clanA", LanceTemplateOriginType.Town),
                LanceWith("wrongOrigin", "empire", "clanA", LanceTemplateOriginType.Mercenary));

            var result = manager.GetLances("empire", "clanA", LanceTemplateOriginType.Town);

            AssertLanceIds(result, "matchAll");
        }

        [TestMethod]
        public void GetLances_ExactCultureMatch_WrongCultureExcluded()
        {
            var manager = CreateManager(
                LanceWith("empireTown", "empire", null, LanceTemplateOriginType.Town),
                LanceWith("vlandiaTown", "vlandia", null, LanceTemplateOriginType.Town));

            var result = manager.GetLances("empire", null, LanceTemplateOriginType.Town);

            AssertLanceIds(result, "empireTown");
        }

        [TestMethod]
        public void GetLances_ExactClanMatch_WrongClanExcluded()
        {
            var manager = CreateManager(
                LanceWith("clanATown", "empire", "clanA", LanceTemplateOriginType.Town),
                LanceWith("clanBTown", "empire", "clanB", LanceTemplateOriginType.Town));

            var result = manager.GetLances("empire", "clanA", LanceTemplateOriginType.Town);

            AssertLanceIds(result, "clanATown");
        }
    }
}
