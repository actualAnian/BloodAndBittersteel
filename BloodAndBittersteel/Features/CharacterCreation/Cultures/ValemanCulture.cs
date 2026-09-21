using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class ValemanCulture : ICharacterCreationCulture
{
    public string CultureId => "valeman";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("valeman_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_valeman_parent_retainer_desc}Your family served in the household of a Vale lord, managing his estates and training the men who guarded his mountain roads and passes.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("valeman_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_valeman_parent_merchant_desc}Your family traded wool, grain, and mountain goods through the market towns and ports of the Vale.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("valeman_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_valeman_parent_farmer_desc}Your family were free smallholders in one of the Vale's fertile valleys and answered their lord's call as part of the levy.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("valeman_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_valeman_parent_blacksmith_desc}Your family kept a smithy in a Vale town, shoeing horses and forging tools, mail, and weapons for the local garrison.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("valeman_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_valeman_parent_hunter_desc}Your family hunted the wooded slopes and high valleys, learning the hidden paths beneath the Mountains of the Moon.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("valeman_education_mountain_page_option", new TextObject("{=bab_valeman_education_mountain_page_title}served as a page in a mountain keep."), new TextObject("{=bab_valeman_education_mountain_page_desc}You were fostered in a high Vale keep, learning courtesy in the hall and discipline from knights who guarded the narrow roads and mountain passes."), (args, u) => u.UpgradeLeadershipTacticsSkills(args, DefaultCharacterAttributes.Social), ccm => IsValeman(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer && !CharacterObject.PlayerCharacter.IsFemale, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
        yield return new NarrativeOption("valeman_education_mountain_lady_option", new TextObject("{=bab_valeman_education_mountain_lady_title}attended a lady of the Vale."), new TextObject("{=bab_valeman_education_mountain_lady_desc}You joined a noble household as a companion and attendant, learning letters, estate management, courtly manners, and the obligations joining the Vale's great families."), (args, u) => u.UpgradeStewardCharmSkills(args, DefaultCharacterAttributes.Social), ccm => IsValeman(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer && CharacterObject.PlayerCharacter.IsFemale, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
        yield return new NarrativeOption("valeman_education_mountain_paths_option", new TextObject("{=bab_valeman_education_mountain_paths_title}learned the high paths."), new TextObject("{=bab_valeman_education_mountain_paths_desc}You travelled with hunters, shepherds, and guides who taught you to read sudden weather, find shelter, and choose safe paths beneath the Mountains of the Moon."), (args, u) => u.UpgradeScoutingTacticsSkills(args, DefaultCharacterAttributes.Cunning), ccm => IsValeman(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Farmer || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Hunter), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_fox", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("valeman_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_valeman_youth_groom_desc}You tended the horses of a landed knight and accompanied him through steep passes and along the high roads of the Vale."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsValeman, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("valeman_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_valeman_youth_cavalry_desc}You trained beside the mounted men-at-arms for which the Vale is famed, learning to keep your seat and couch a lance."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsValeman, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("valeman_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_valeman_youth_guard_desc}You stood watch over a mountain stronghold, where a narrow path and a stout gate could hold back an army."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsValeman, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("valeman_youth_bandit_option", new TextObject("{=bab_valeman_youth_bandit_title}ran with a mountain band."), new TextObject("{=bab_valeman_youth_bandit_desc}You lived among outlaws in the high valleys, learning to move along goat tracks and strike from the rocks before vanishing into the mountains."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsValeman, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("valeman_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_valeman_youth_infantry_desc}You drilled with the spearmen and swordsmen who held the Vale's passes when mounted knights could go no farther."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsValeman, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("valeman_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_valeman_youth_skirmisher_desc}You learned to harry foes from broken ground, loosing missiles from slopes where heavier soldiers struggled to follow."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsValeman, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsValeman(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "valeman";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsValeman, ccm =>
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

    private const string CultureIdValue = "valeman";
}
