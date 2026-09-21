using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class DornishCulture : ICharacterCreationCulture
{
    public string CultureId => "dornish";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("dornish_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_dornish_parent_retainer_desc}Your family served a Dornish lord, administering his holdings and keeping riders ready to defend well, pass, and holdfast.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("dornish_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_dornish_parent_merchant_desc}Your family traded wine, fruit, spices, and cloth along Dorne's roads and through its ports.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("dornish_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_dornish_parent_farmer_desc}Your family tended irrigated fields and orchards, guarding every drop of water and answering the local lord's summons.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("dornish_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_dornish_parent_blacksmith_desc}Your family ran a smithy in a Dornish town, making tools for field and vineyard as well as arms for guards and riders.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("dornish_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_dornish_parent_hunter_desc}Your family hunted the stony hills and dry valleys, reading faint tracks and surviving far from any well.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("dornish_education_water_gardens_option", new TextObject("{=bab_dornish_education_water_gardens_title}were fostered with children of every station."), new TextObject("{=bab_dornish_education_water_gardens_desc}You spent part of your youth among other Dornish children under noble protection, learning that prince, merchant, craftsman, and peasant all depended upon the same land and water."), (args, u) => u.UpgradeLeadershipCharmSkills(args, DefaultCharacterAttributes.Social), ccm => IsDornish(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.MerchantUrban || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.ArtisanUrban || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Farmer), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
        yield return new NarrativeOption("dornish_education_dornish_heir_option", new TextObject("{=bab_dornish_education_dornish_heir_title}trained beside the household's heirs."), new TextObject("{=bab_dornish_education_dornish_heir_desc}Dornish custom placed fewer limits upon daughters, and you were taught beside the household's young heirs to ride, judge disputes, and defend the family's rights."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), ccm => IsDornish(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer && CharacterObject.PlayerCharacter.IsFemale, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_apprentice", "", ""));
        yield return new NarrativeOption("dornish_education_water_steward_option", new TextObject("{=bab_dornish_education_water_steward_title}helped measure wells and watercourses."), new TextObject("{=bab_dornish_education_water_steward_desc}In Dorne, a neglected cistern or stolen flow of water could ruin a holding. You learned to inspect channels, ration stores, and settle quarrels over irrigation."), (args, u) => u.UpgradeStewardTacticsSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsDornish(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Farmer || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.ArtisanUrban), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_grit", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("dornish_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_dornish_youth_groom_desc}You tended a Dornish knight's swift horse and carried messages along dusty roads beneath the hot sun."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsDornish, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("dornish_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_dornish_youth_cavalry_desc}You trained with light and heavy riders, learning to conserve your mount before closing suddenly with spear and sword."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsDornish, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("dornish_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_dornish_youth_guard_desc}You guarded a desert holdfast, watching wells and gates and learning how stone walls could master heat and distance."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsDornish, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("dornish_youth_bandit_option", new TextObject("{=bab_dornish_youth_bandit_title}rode with an outlaw band."), new TextObject("{=bab_dornish_youth_bandit_desc}You rode with smugglers and outlaws who knew hidden springs, lonely tracks, and ways around a lord's patrols."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsDornish, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("dornish_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_dornish_youth_infantry_desc}You drilled with spear and shield to hold narrow passes and punish any foe exhausted by the Dornish heat."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsDornish, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("dornish_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_dornish_youth_skirmisher_desc}You learned to strike at range, withdraw across harsh ground, and draw heavier enemies into a fruitless pursuit."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsDornish, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsDornish(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "dornish";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsDornish, ccm =>
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

    private const string CultureIdValue = "dornish";
}
