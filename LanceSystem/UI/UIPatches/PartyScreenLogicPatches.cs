using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;

namespace LanceSystem.UI.UIPatches
{
    [HarmonyPatch(typeof(PartyScreenLogic), nameof(PartyScreenLogic.DoneLogic))]
    internal class PartyScreenLogicDonePatch
    {
        public static bool Prefix()
        {
            LancePartyVM.Instance?.OnDone();

            return true;
        }
    }
}
