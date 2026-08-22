using LanceSystem.SimpleFuzzySearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops
{
    public static class TempCheat
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("create_character", "bab")]
        public static string CreateCharacter(List<string> args)
        {
            string characterName = "Test Character";
            var culture = MBObjectManager.Instance.GetObject<CultureObject>("empire");
            var upgradeTarget = MBObjectManager.Instance.GetObject<CharacterObject>("imperial_equite");
            DynamicTroopsService.Instance.CreateCharacterFromData(characterName, false, FormationClass.Infantry, 1, culture, new List<CharacterObject>() { upgradeTarget }, new MBEquipmentRoster(), null);
            //TaleWorlds.CampaignSystem.Party.MobileParty.MainParty.AddElementToMemberRoster(test, 1);
            return $"Character created: {characterName}";
        }
    }
}
