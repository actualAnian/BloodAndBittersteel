using LanceSystem.LanceDataClasses;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace LanceSystem
{
    public class LanceEvents
    {
        private static readonly MbEvent<LanceData> _lanceDisbanded = new();
        public static IMbEvent<LanceData> LanceDisbanded
        {
            get
            {
                return _lanceDisbanded;
            }
        }
        internal static void OnLanceDisbanded(LanceData lance)
        {
            _lanceDisbanded.Invoke(lance);
        }
        private static readonly MbEvent<PartyBase, CharacterObject, CharacterObject, int> _aiUpgradeTroops = new();
        public static IMbEvent<PartyBase, CharacterObject, CharacterObject, int> AiUpgradeTroops
        {
            get
            {
                return _aiUpgradeTroops;
            }
        }
        internal static void OnAiUpgradeTroops(PartyBase party, CharacterObject from, CharacterObject to, int amount)
        {
            _aiUpgradeTroops.Invoke(party, from, to, amount);
        }
    }
}