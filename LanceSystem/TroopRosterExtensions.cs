using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace LanceSystem
{
    internal static class TroopRosterExtensions
    {
        public static float CalculateTroopRosterStrength(this TroopRoster roster, BattleSideEnum side, MapEvent.PowerCalculationContext context)
        {
            float value = 0;
            foreach(var troop in roster.GetTroopRoster())
                if (troop.Character != null)
                {
                    float troopPower = Campaign.Current.Models.MilitaryPowerModel.GetTroopPower(troop.Character, side, context, 0);
                    value += (troop.Number - troop.WoundedNumber) * troopPower;
                }
            return value;
        }
    }
}
