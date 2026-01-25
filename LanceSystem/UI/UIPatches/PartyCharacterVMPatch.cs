using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;

namespace LanceSystem.UI.UIPatches
{
    internal class PartyCharacterVMPatch
    {
        static readonly FieldInfo _partyVM = AccessTools.Field("PartyCharacterVM:_partyVm");
        //[HarmonyPatch(typeof(PartyCharacterVM), nameof(PartyCharacterVM.Upgrade))]
        public static bool Prefix(PartyCharacterVM __instance, int upgradeIndex, int maxUpgradeCount)
        {
            var partyVM = _partyVM.GetValue(__instance);
            if (partyVM is LancePartyVM babPartyVM)
            {
                babPartyVM.ExecuteUpgrade(__instance, upgradeIndex, maxUpgradeCount);
                return false;
            }
            return true;
        }
    }
    internal class PartyCharacterVMRecruitPrisonerPatch
    {
        static readonly FieldInfo _partyVM = AccessTools.Field("PartyCharacterVM:_partyVm");
        //[HarmonyPatch(typeof(PartyCharacterVM), nameof(PartyCharacterVM.ExecuteRecruitTroop))]
        public static bool Prefix(PartyCharacterVM __instance)
        {
            var partyVM = _partyVM.GetValue(__instance);
            if (partyVM is LancePartyVM babPartyVM)
            {
                babPartyVM.ExecuteRecruit(__instance, false);
                return false;
            }
            return true;
        }
    }

}
