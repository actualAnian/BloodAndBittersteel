using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace LanceSystem.DynamicLances.UI
{
    public static class LanceTemplateEditorCommands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("open_lance_editor", "bab")]
        public static string OpenLanceEditor(List<string> args)
        {
            if (Campaign.Current == null) return "Campaign not loaded - load a save first";
            //string lanceId = args != null && args.Count > 0 ? args[0] : null;
            //if (string.IsNullOrWhiteSpace(lanceId))
            //{
            //    LanceTemplateEditorManager.CreateLayer();
            //    return "Opened lance template editor (new template)";
            //}
            //if (!LanceTemplateManager.Instance.Lances.ContainsKey(lanceId)) return "Lance not found: " + lanceId;
            var lanceId = "aserai_all";
            LanceTemplateEditorManager.CreateLayer(lanceId);
            return "Opened lance template editor for " + lanceId;
        }
    }
}
