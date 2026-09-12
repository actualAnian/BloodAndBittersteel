using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation;

public static class MainlandChildhoodOptions
{
    public static readonly IReadOnlyList<NarrativeOption> Shared = new NarrativeOption[]
    {
        new NarrativeOption("childhood_leadership_option", new TextObject("{=kmM68Qx4}your leadership skills."), new TextObject("{=FfNwXtii}If the wolf pup gang of your early childhood had an alpha, it was definitely you. All the other kids followed your lead as you decided what to play and where to play, and led them in games and mischief."), (args, u) => u.UpgradeLeadershipTacticsSkills(args, DefaultCharacterAttributes.Cunning), _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_childhood_character", "act_childhood_leader")),
        new NarrativeOption("childhood_brawn_option", new TextObject("{=5HXS8HEY}your brawn."), new TextObject("{=YKzuGc54}You were big, and other children looked to have you around in any scrap with children from a neighboring village. You pushed a plough and threw an axe like an adult."), (args, u) => u.UpgradeTwoHandedBowSkills(args, DefaultCharacterAttributes.Vigor), _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_childhood_character", "act_childhood_athlete")),
        new NarrativeOption("childhood_detail_option", new TextObject("{=QrYjPUEf}your attention to detail."), new TextObject("{=JUSHAPnu}You were quick on your feet and attentive to what was going on around you. Usually you could run away from trouble, though you could give a good account of yourself in a fight with other children if cornered."), (args, u) => u.UpgradeAthleticsBowSkills(args, DefaultCharacterAttributes.Control), _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_childhood_character", "act_childhood_memory")),
        new NarrativeOption("childhood_smart_option", new TextObject("{=Y3UcaX74}your aptitude for numbers."), new TextObject("{=DFidSjIf}Most children around you had only the most rudimentary education, but you lingered after class to study letters and mathematics. You were fascinated by the marketplace - weights and measures, tallies and accounts, the chatter about profits and losses."), (args, u) => u.UpgradeEngineeringTradeSkills(args, DefaultCharacterAttributes.Intelligence), _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_childhood_character", "act_childhood_numbers")),
        new NarrativeOption("childhood_leader_option", new TextObject("{=GEYzLuwb}your way with people."), new TextObject("{=w2TEQq26}You were always attentive to other people, good at guessing their motivations. You studied how individuals were swayed, and tried out what you learned from adults on your friends."), (args, u) => u.UpgradeLeadershipCharmSkills(args, DefaultCharacterAttributes.Social), _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_childhood_character", "act_childhood_manners")),
        new NarrativeOption("childhood_horse_option", new TextObject("{=MEgLE2kj}your skill with horses."), new TextObject("{=ngazFofr}You were always drawn to animals, and spent as much time as possible hanging out in the village stables. You could calm horses, and were sometimes called upon to break in new colts. You learned the basics of veterinary arts, much of which is applicable to humans as well."), (args, u) => u.UpgradeRidingMedicineSkills(args, DefaultCharacterAttributes.Endurance), _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_childhood_character", "act_childhood_animals")),
    };
}
