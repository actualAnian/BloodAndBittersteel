using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace LanceSystem.Patches
{
    [HarmonyPatch(typeof(PartyUpgraderCampaignBehavior), "UpgradeTroop")]
    public static class OnTroopUpgraded
    {
        public static void Postfix(PartyBase party, int rosterIndex, object upgradeArgs)
        {
            var type = upgradeArgs.GetType();

            var fromField = type.GetField("Target", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var from = (CharacterObject)fromField.GetValue(upgradeArgs);
            var toField = type.GetField("UpgradeTarget", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var to = (CharacterObject)toField.GetValue(upgradeArgs);
            var PossibleUpgradeCountField = type.GetField("PossibleUpgradeCount", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var amount = (int)PossibleUpgradeCountField.GetValue(upgradeArgs);
            LanceEvents.OnAiUpgradeTroops(party, from, to, amount);
        }
    }
}
