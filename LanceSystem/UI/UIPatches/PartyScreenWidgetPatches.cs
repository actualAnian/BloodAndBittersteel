using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;

namespace LanceSystem.UI.UIPatches
{
    [HarmonyPatch(typeof(PartyScreenWidget), nameof(PartyScreenWidget.MainMemberList), MethodType.Getter)]
    public static class PartyScreenWidgetPatches2
    {
        static void Prefix(PartyScreenWidget __instance)
        {
            __instance.MainMemberList = (PartyListPanel)__instance.MainPrisonerList.FindParentPanel().Children[1].Children[0].Children[0].Children[0].Children[1].Children[0];
            //__instance.MainMemberList = (PartyListPanel)value.FindParentPanel().Children[1].Children[0].Children[0].Children[1].Children[0];
        }
    }
}
