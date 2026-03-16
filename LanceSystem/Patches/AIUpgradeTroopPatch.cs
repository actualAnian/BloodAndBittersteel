using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using static TaleWorlds.CampaignSystem.CampaignBehaviors.RecruitmentCampaignBehavior;

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
            var PossibleUpgradeCountField = type.GetField("Target", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var amount = (int)PossibleUpgradeCountField.GetValue(upgradeArgs);
            LanceEvents.OnAiUpgradeTroops(party, from, to, amount);
        }
    }
}
