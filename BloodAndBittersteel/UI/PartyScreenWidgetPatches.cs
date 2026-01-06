using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;

namespace BloodAndBittersteel.UI
{
    [HarmonyPatch(typeof(PartyScreenWidget), nameof(PartyScreenWidget.MainPrisonerList), MethodType.Setter)]
    public static class PartyScreenWidgetPatches
    {
        static void Postfix(PartyScreenWidget __instance, ListPanel value)
        {
            __instance.MainMemberList = (PartyListPanel)value.FindParentPanel().Children[1].Children[0].Children[0].Children[1].Children[0];    
        }
    }
}
