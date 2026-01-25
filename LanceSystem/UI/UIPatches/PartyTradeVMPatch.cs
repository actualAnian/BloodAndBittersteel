using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using static TaleWorlds.CampaignSystem.Party.PartyScreenLogic;

namespace LanceSystem.UI.UIPatches
{
    public class PartyTradeVMPatch
    {
        static FieldInfo isPrisoner = AccessTools.Field("PartyTradeVM:_isPrisoner");
        [HarmonyPatch(typeof(PartyTradeVM), nameof(PartyTradeVM.UpdateTroopData))]
        public static bool Prefix(PartyTradeVM __instance, TroopRosterElement troopRoster, PartyRosterSide side, bool forceUpdate = true)
        {
            if (LancePartyVM.Instance != null
                || (bool)isPrisoner.GetValue(__instance)) return false;
            return true;
            //if ((bool)method.GetValue(__instance)) return true;
            //if (side == PartyRosterSide.Right)
            //{
            //    CharacterObject troop = troopRoster.Character;
            //    __instance.InitialThisStock = troopRoster.Number;
            //    __instance.ThisStock = __instance.InitialThisStock;
            //    int index = BaBPartyVM.Instance.PartyScreenLogic.MemberRosters[0].FindIndexOfTroop(troop);
            //    if (index != -1) 
            //    {
            //        var otherNumber = BaBPartyVM.Instance.PartyScreenLogic.MemberRosters[0].GetElementNumber(index);
            //        __instance.InitialOtherStock = otherNumber;
            //        __instance.OtherStock = __instance.InitialOtherStock;
            //    }
            //    else
            //    {
            //        __instance.InitialOtherStock = 0;
            //        __instance.OtherStock = __instance.InitialOtherStock;

            //    }
            //}
            //return true;
        }
    }
}