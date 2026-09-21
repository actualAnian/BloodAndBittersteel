using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class NorthmanCulture : ICharacterCreationCulture
{
    public string CultureId => "northman";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("northman_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_northman_parent_retainer_desc}Your family served a northern lord, keeping his hall and lands through long winters and mustering his folk when called.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("northman_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_northman_parent_merchant_desc}Your family traded wool, timber, hides, and preserved food between scattered northern holds and market towns.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("northman_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_northman_parent_farmer_desc}Your family worked a northern holding, storing what the short summers yielded and serving in the levy when required.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("northman_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_northman_parent_blacksmith_desc}Your family kept a forge whose tools, nails, weapons, and horseshoes were essential to an isolated northern community.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("northman_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_northman_parent_hunter_desc}Your family ranged through deep forests and snowy hills, trapping fur and bringing down game through the lean months.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("northman_education_northern_ward_option", new TextObject("{=bab_northman_education_northern_ward_title}were fostered in another northern hall."), new TextObject("{=bab_northman_education_northern_ward_desc}You were sent to another lord's hall as a ward, sharing lessons, meals, work, and training with the household's children until ties of duty became personal loyalty."), (args, u) => u.UpgradeLeadershipTacticsSkills(args, DefaultCharacterAttributes.Social), ccm => IsNorthman(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
        yield return new NarrativeOption("northman_education_winter_stores_option", new TextObject("{=bab_northman_education_winter_stores_title}prepared the holding for winter."), new TextObject("{=bab_northman_education_winter_stores_desc}You counted grain, smoked meat, dried vegetables, cut peat, and inspected roofs and cellars, learning that poor stewardship could kill more surely than any sword."), (args, u) => u.UpgradeLeadershipStewardSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsNorthman(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Farmer || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.MerchantUrban), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_grit", "", ""));
        yield return new NarrativeOption("northman_education_deep_woods_option", new TextObject("{=bab_northman_education_deep_woods_title}learned the deep northern woods."), new TextObject("{=bab_northman_education_deep_woods_desc}You travelled through pine forest, bog, and snow, learning to follow game, read weather, find shelter, and return safely when familiar paths disappeared."), (args, u) => u.UpgradeScoutingBowSkills(args, DefaultCharacterAttributes.Cunning), ccm => IsNorthman(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Hunter, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_sharp", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("northman_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_northman_youth_groom_desc}You tended a northern lord's horses and carried his messages over long roads through forest, snow, and cold rain."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsNorthman, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("northman_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_northman_youth_cavalry_desc}You trained with mounted retainers, learning to fight from a hardy horse bred for distance and difficult ground."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsNorthman, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("northman_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_northman_youth_guard_desc}You guarded a timber hall or ancient stone keep and learned the patience required for lonely watches in bitter weather."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsNorthman, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("northman_youth_bandit_option", new TextObject("{=bab_northman_youth_bandit_title}ran with a band of broken men."), new TextObject("{=bab_northman_youth_bandit_desc}You survived with poachers and deserters in the vast northern woods, where distance and winter hid you from pursuit."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsNorthman, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("northman_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_northman_youth_infantry_desc}You drilled with shield, spear, and heavy blade in the stubborn ranks that form the strength of a northern host."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsNorthman, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("northman_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_northman_youth_skirmisher_desc}You learned to scout through forest and snow, striking from cover before leading enemies onto hostile ground."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsNorthman, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
    }

    public IEnumerable<NarrativeOption> GetAdulthoodOptions()
    {
        yield return AdulthoodCultureOptions.DefeatedEnemy;
        yield return AdulthoodCultureOptions.Manhunt;
        yield return AdulthoodCultureOptions.CaravanLeader;
        yield return AdulthoodCultureOptions.Workshop;
        yield return AdulthoodCultureOptions.Investor;
        yield return AdulthoodCultureOptions.Hunter;
        yield return AdulthoodCultureOptions.SiegeSurvivor;
        yield return AdulthoodCultureOptions.EscapadeHigh;
        yield return AdulthoodCultureOptions.EscapadeLow;
        yield return AdulthoodCultureOptions.NicePerson;
    }

    private static bool IsNorthman(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "northman";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsNorthman, ccm =>
        {
            ccm.CharacterCreationContent.SetParentOccupation(occupationType);
            string cultureId = ccm.CharacterCreationContent.SelectedCulture.StringId;
            string motherEquipmentId = NarrativeEquipmentHelper.GetMotherEquipmentId(ccm, occupationType, cultureId);
            string fatherEquipmentId = NarrativeEquipmentHelper.GetFatherEquipmentId(ccm, occupationType, cultureId);
            NarrativeEquipmentHelper.SetParentEquipment(ccm, motherEquipmentId, fatherEquipmentId, motherAnimation, fatherAnimation);
        });
    }

    private static void SelectYouth(CharacterCreationManager ccm, string titleType, string animation)
    {
        ccm.CharacterCreationContent.SelectedTitleType = titleType;
        string playerEquipmentId = NarrativeEquipmentHelper.GetPlayerEquipmentId(ccm, titleType, CultureIdValue, Hero.MainHero.IsFemale);
        NarrativeEquipmentHelper.SetPlayerEquipment(ccm, playerEquipmentId, animation);
    }

    private const string CultureIdValue = "northman";
}
