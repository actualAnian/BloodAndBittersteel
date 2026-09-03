using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.CampaignSystem;
using LanceSystem.DynamicTroops.UI.Services;

namespace LanceSystem.DynamicTroops.UI
{
    public static class TroopEditorCommands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("open_troop_editor", "bab")]
        public static string OpenTroopEditor(List<string> args)
        {
            string troopId = args != null && args.Count > 0 ? args[0] : "imperial_recruit";
            CharacterObject troop = MBObjectManager.Instance.GetObject<CharacterObject>(troopId);
            if (troop == null && Game.Current != null) troop = Game.Current.ObjectManager.GetObject<CharacterObject>(troopId);
            if (troop == null) return "Troop not found: " + troopId;
            if (Campaign.Current == null) return "Campaign not loaded - load a save first";
            TroopEditorController manager = new TroopEditorController(troop);
            TroopEditorViewService.Create(manager.Vm);
            return "Opened troop editor for " + troop.StringId;
        }
    }
}