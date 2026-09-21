using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class StormlanderCulture : ICharacterCreationCulture
{
    public string CultureId => "stormlander";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("stormlander_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_stormlander_parent_retainer_desc}Your family served a storm lord, overseeing his household and mustering the hard folk of the rainwood when banners were called.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("stormlander_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_stormlander_parent_merchant_desc}Your family dealt in timber, grain, and goods landed at the storm coast's guarded harbours.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("stormlander_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_stormlander_parent_farmer_desc}Your family worked stubborn soil beneath frequent storms and stood in the levy when raiders or rival lords threatened.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("stormlander_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_stormlander_parent_blacksmith_desc}Your family owned a town smithy and forged the sturdy tools and weapons demanded by life in the stormlands.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("stormlander_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_stormlander_parent_hunter_desc}Your family hunted beneath the dark boughs of the rainwood and knew how to endure its downpours and tangled paths.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("stormlander_education_storm_keep_option", new TextObject("{=bab_stormlander_education_storm_keep_title}served in a storm lord's household."), new TextObject("{=bab_stormlander_education_storm_keep_desc}You learned your duties in a rain-beaten castle, carrying orders between hall, stable, armoury, and gate while storms tested every roof and wall."), (args, u) => u.UpgradeLeadershipTacticsSkills(args, DefaultCharacterAttributes.Social), ccm => IsStormlander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
        yield return new NarrativeOption("stormlander_education_rainwood_option", new TextObject("{=bab_stormlander_education_rainwood_title}learned the ways of the rainwood."), new TextObject("{=bab_stormlander_education_rainwood_desc}You ranged beneath the wet green canopy, learning to follow game, cross swollen streams, keep bowstrings dry, and find paths hidden by rain and undergrowth."), (args, u) => u.UpgradeScoutingBowSkills(args, DefaultCharacterAttributes.Cunning), ccm => IsStormlander(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Hunter || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Farmer), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_sharp", "", ""));
        yield return new NarrativeOption("stormlander_education_castle_supplies_option", new TextObject("{=bab_stormlander_education_castle_supplies_title}provisioned castles against the storms."), new TextObject("{=bab_stormlander_education_castle_supplies_desc}You helped merchants and craftsmen move grain, timber, ironwork, and lamp oil into guarded storehouses before roads and harbours were closed by violent weather."), (args, u) => u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsStormlander(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.MerchantUrban || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.ArtisanUrban), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_peddlers_2", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("stormlander_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_stormlander_youth_groom_desc}You served a landed knight, caring for his destrier and carrying orders through driving rain and sodden roads."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsStormlander, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("stormlander_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_stormlander_youth_cavalry_desc}You trained with mounted men-at-arms, learning to charge over rough ground even when storms turned the fields to mud."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsStormlander, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("stormlander_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_stormlander_youth_guard_desc}You guarded a rain-beaten castle and learned to keep bowstrings dry, walls supplied, and watchfires burning."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsStormlander, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("stormlander_youth_bandit_option", new TextObject("{=bab_stormlander_youth_bandit_title}ran with an outlaw band."), new TextObject("{=bab_stormlander_youth_bandit_desc}You joined broken men hidden in the rainwood, surviving by poaching, ambush, and secret paths beneath the trees."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsStormlander, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("stormlander_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_stormlander_youth_infantry_desc}You trained among the stormlands' foot, learning to hold a shield wall against raiders and rival hosts."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsStormlander, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("stormlander_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_stormlander_youth_skirmisher_desc}You ranged ahead through rain and woodland, striking quickly with bow and javelin before slipping back into cover."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsStormlander, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsStormlander(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "stormlander";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsStormlander, ccm =>
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

    private const string CultureIdValue = "stormlander";
}
