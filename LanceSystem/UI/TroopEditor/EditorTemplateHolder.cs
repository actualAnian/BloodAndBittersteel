using LanceSystem.Deserialization;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.UI.TroopEditor
{
    public class EditorTemplateHolder
    {
        static EditorTemplateHolder? _instance;
        public static EditorTemplateHolder Instance => _instance ??= new EditorTemplateHolder();
        public Lance? LanceTemplate { get; set; }
        public CharacterObject? TroopPreviewCharacter { get; set; }
    }
}
