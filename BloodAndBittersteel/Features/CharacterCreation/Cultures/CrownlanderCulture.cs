using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class CrownlanderCulture : ICharacterCreationCulture
{
    public string CultureId => "crownlander";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("crownlander_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_crownlander_parent_retainer_desc}Your family served in the household of a Crownlands lord. They managed his lands, settled disputes among his smallfolk, and trained the local levy. When called to war, they rode beneath his banner.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("crownlander_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_crownlander_parent_merchant_desc}Your family were merchants in one of the busy towns of the Crownlands. They organized caravans, dealt with ships and traders from across the narrow sea, and were active in the local merchants' guild.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("crownlander_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_crownlander_parent_farmer_desc}Your family were free smallholders with enough land to feed themselves and sell a modest surplus. They were pillars of the local economy and formed the backbone of their lord's levy.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("crownlander_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_crownlander_parent_blacksmith_desc}Your family owned a smithy in a Crownlands town. Your father played a small part in the town's affairs and served with the watch when danger threatened.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("crownlander_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_crownlander_parent_hunter_desc}Your family lived in a village but owned little land. Your father supplemented paid work with long trips through the kingswood, hunting and trapping while keeping a wary eye out for the royal foresters.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions() => MainlandEducationOptions.Shared;

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("crownlander_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_crownlander_youth_groom_desc}You accompanied a landed knight in the service of a Crownlands lord. You carried messages and tended horses, gaining a close view of how campaigns were planned and men deployed in battle."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsCrownlander, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("crownlander_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_crownlander_youth_cavalry_desc}You could never have afforded the equipment yourself, but your skill in the saddle persuaded a local lord to lend you a horse and arms. You trained with his mounted men-at-arms and learned to fight with the lance."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsCrownlander, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("crownlander_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_crownlander_youth_guard_desc}The towns and castles of the Crownlands always needed watchmen. You learned to patrol walls, keep order at the gates, and defend the battlements in time of siege."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsCrownlander, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("crownlander_youth_bandit_option", new TextObject("{=bab_crownlander_youth_bandit_title}ran with an outlaw band."), new TextObject("{=bab_crownlander_youth_bandit_desc}You fell in with poachers, smugglers, and broken men who haunted the roads and forests beyond the reach of a lord's justice. You learned to move unseen and survive by your wits."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsCrownlander, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("crownlander_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_crownlander_youth_infantry_desc}Spearmen and swordsmen drawn from the Crownlands smallfolk form the bulk of many a lord's host. You drilled to stand in formation and hold your ground beneath a rain of arrows."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsCrownlander, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("crownlander_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_crownlander_youth_skirmisher_desc}You joined the lightly armed youths who screened the main host. With bow and javelin, you learned to harry the enemy and withdraw before heavier troops could close with you."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsCrownlander, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsCrownlander(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "crownlander";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsCrownlander, ccm =>
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

    private const string CultureIdValue = "crownlander";
}
