using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace LanceSystem.Models
{
    public class LanceTavernMercenaryTroopsModel : TavernMercenaryTroopsModel
    {
        public override float RegularMercenariesSpawnChance
        {
            get
            {
                return 0f;
            }
        }
    }
}
