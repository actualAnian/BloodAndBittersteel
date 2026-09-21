using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class ReachmanCulture : ICharacterCreationCulture
{
    public string CultureId => "reachman";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("reachman_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_reachman_parent_retainer_desc}Your family served in a Reach lord's household, keeping his rich estates productive and his numerous levies prepared.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("reachman_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_reachman_parent_merchant_desc}Your family traded grain, wine, fruit, and fine goods between the Reach's prosperous towns and river ports.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("reachman_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_reachman_parent_farmer_desc}Your family were prosperous smallholders in the fertile Reach, raising abundant crops and serving in the local levy.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("reachman_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_reachman_parent_blacksmith_desc}Your family owned a busy smithy, supplying farms with tools and knights and men-at-arms with weapons and horseshoes.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("reachman_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_reachman_parent_hunter_desc}Your family hunted the Reach's woods and managed game for nearby estates while avoiding the wrath of noble foresters.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("reachman_education_tourney_page_option", new TextObject("{=bab_reachman_education_tourney_page_title}served at tourneys and noble musters."), new TextObject("{=bab_reachman_education_tourney_page_desc}You waited upon knights during tourneys and musters, caring for horses and harness while observing how reputation, courtesy, and skill at arms advanced a young retainer."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), ccm => IsReachman(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer && !CharacterObject.PlayerCharacter.IsFemale, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_apprentice", "", ""));
        yield return new NarrativeOption("reachman_education_reach_lady_option", new TextObject("{=bab_reachman_education_reach_lady_title}joined a great lady's household."), new TextObject("{=bab_reachman_education_reach_lady_desc}You served among a noblewoman's companions, learning music, letters, household accounts, and the alliances and rivalries concealed beneath courtly courtesy."), (args, u) => u.UpgradeStewardCharmSkills(args, DefaultCharacterAttributes.Social), ccm => IsReachman(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer && CharacterObject.PlayerCharacter.IsFemale, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
        yield return new NarrativeOption("reachman_education_estate_granaries_option", new TextObject("{=bab_reachman_education_estate_granaries_title}helped manage the estate granaries."), new TextObject("{=bab_reachman_education_estate_granaries_desc}The Reach's abundance required careful hands. You learned to reckon seed, rents, harvests, storage losses, and the wagons needed to feed a lord's household and host."), (args, u) => u.UpgradeLeadershipStewardSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsReachman(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Farmer || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.MerchantUrban), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_book", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("reachman_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_reachman_youth_groom_desc}You served a landed knight amid the Reach's tourneys and musters, tending costly horses and polished harness."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsReachman, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("reachman_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_reachman_youth_cavalry_desc}You trained with the Reach's celebrated horsemen, learning the disciplined charge and the demanding use of the lance."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsReachman, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("reachman_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_reachman_youth_guard_desc}You stood guard over a wealthy town or castle, protecting granaries, gates, and storehouses from thieves and enemies."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsReachman, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("reachman_youth_bandit_option", new TextObject("{=bab_reachman_youth_bandit_title}ran with an outlaw band."), new TextObject("{=bab_reachman_youth_bandit_desc}You hid among hedgerows and wooded riverbanks with poachers and broken men who preyed upon the Reach's busy roads."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsReachman, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("reachman_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_reachman_youth_infantry_desc}You drilled in the dense ranks of spearmen and billmen who supported the Reach's splendid mounted knights."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsReachman, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("reachman_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_reachman_youth_skirmisher_desc}You screened the host with bow and javelin, using fields, ditches, and hedges to cover your retreat."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsReachman, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsReachman(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "reachman";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsReachman, ccm =>
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

    private const string CultureIdValue = "reachman";
}
