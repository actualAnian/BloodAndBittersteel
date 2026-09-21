using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class RiverlanderCulture : ICharacterCreationCulture
{
    public string CultureId => "riverlander";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("riverlander_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_riverlander_parent_retainer_desc}Your family served a river lord, settling disputes among tenants and mustering men to defend ford, ferry, and field.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("riverlander_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_riverlander_parent_merchant_desc}Your family carried grain, fish, timber, and wool along the river roads and waterways of the Trident.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("riverlander_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_riverlander_parent_farmer_desc}Your family farmed the rich but often-contested soil of the riverlands and learned to rebuild after passing armies.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("riverlander_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_riverlander_parent_blacksmith_desc}Your family owned a town forge, repairing ploughs in peace and making spearheads and horseshoes whenever war returned.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("riverlander_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_riverlander_parent_hunter_desc}Your family hunted beside rivers, marshes, and woods, becoming skilled trackers and boatmen.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("riverlander_education_ferry_accounts_option", new TextObject("{=bab_riverlander_education_ferry_accounts_title}collected tolls at a ferry or bridge."), new TextObject("{=bab_riverlander_education_ferry_accounts_desc}You helped reckon cargoes, collect lawful tolls, and judge when flood or war made a crossing too dangerous for travellers and wagons."), (args, u) => u.UpgradeEngineeringTradeSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsRiverlander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.MerchantUrban, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_book", "", ""));
        yield return new NarrativeOption("riverlander_education_river_works_option", new TextObject("{=bab_riverlander_education_river_works_title}maintained dikes, mills, and watercourses."), new TextObject("{=bab_riverlander_education_river_works_desc}You laboured where river met field, learning how banks, sluices, millraces, and drainage ditches protected crops and kept a community fed."), (args, u) => u.UpgradeCraftingEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsRiverlander(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Farmer || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.ArtisanUrban), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_grit", "", ""));
        yield return new NarrativeOption("riverlander_education_river_ward_option", new TextObject("{=bab_riverlander_education_river_ward_title}were fostered in another river lord's hall."), new TextObject("{=bab_riverlander_education_river_ward_desc}Your family sent you to a neighbouring hall, where service beside other wards taught you courtesy, local loyalties, and how quickly friendship could become alliance in the riverlands."), (args, u) => u.UpgradeLeadershipCharmSkills(args, DefaultCharacterAttributes.Social), ccm => IsRiverlander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("riverlander_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_riverlander_youth_groom_desc}You served a landed knight and carried his messages across ferries, muddy lanes, and the many branches of the Trident."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsRiverlander, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("riverlander_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_riverlander_youth_cavalry_desc}You trained with mounted men-at-arms, learning where horses could ford safely and where wet ground would ruin a charge."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsRiverlander, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("riverlander_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_riverlander_youth_guard_desc}You guarded a bridge, ferry, or riverside castle whose possession could decide an entire campaign."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsRiverlander, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("riverlander_youth_bandit_option", new TextObject("{=bab_riverlander_youth_bandit_title}ran with an outlaw band."), new TextObject("{=bab_riverlander_youth_bandit_desc}You lived among broken men in willow woods and marshy islands, striking at travellers before escaping across the water."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsRiverlander, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("riverlander_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_riverlander_youth_infantry_desc}You trained with spear and shield, prepared to defend village and ford from yet another army crossing the riverlands."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsRiverlander, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("riverlander_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_riverlander_youth_skirmisher_desc}You learned to fight from riverbanks and thickets, harrying enemies across ground you knew better than they did."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsRiverlander, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsRiverlander(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "riverlander";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsRiverlander, ccm =>
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

    private const string CultureIdValue = "riverlander";
}
