using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops.UI
{
    public record TroopEditorData
    {
        public string Name { get; init; } = "";
        public bool IsFemale { get; init; }
        public FormationClass DefaultGroup { get; init; }
        public int Tier { get; init; }
        public CultureObject Culture { get; init; } = default!;
        public List<CharacterObject> UpgradesTo { get; init; } = new();
        public MBEquipmentRoster Roster { get; init; } = default!;
        public MBBodyProperty? FaceKeyTemplate { get; init; }
    }
}
