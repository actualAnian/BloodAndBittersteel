using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class WesterlanderCulture : ICharacterCreationCulture
{
    public string CultureId => "westerlander";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("westerlander_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_westerlander_parent_retainer_desc}Your family served a Westerlands lord, supervising mines, estates, and tenants while helping train his household levy.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("westerlander_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_westerlander_parent_merchant_desc}Your family traded ore, metalwork, wool, and luxury goods through the wealthy towns of the Westerlands.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("westerlander_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_westerlander_parent_farmer_desc}Your family held a modest farm among rocky hills and fertile valleys, owing service to a powerful local lord.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("westerlander_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_westerlander_parent_blacksmith_desc}Your family worked metal in a prosperous town, with fine ore and wealthy patrons bringing steady custom to the forge.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("westerlander_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_westerlander_parent_hunter_desc}Your family hunted the wooded hills, trapped valuable pelts, and learned the forgotten tracks between mines and keeps.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("westerlander_education_mine_accounts_option", new TextObject("{=bab_westerlander_education_mine_accounts_title}kept accounts for mines and metal traders."), new TextObject("{=bab_westerlander_education_mine_accounts_desc}You recorded ore, wages, timber, tools, and shipments, learning how the wealth beneath the hills passed through many hands before becoming coin."), (args, u) => u.UpgradeEngineeringTradeSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsWesterlander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.MerchantUrban, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_book", "", ""));
        yield return new NarrativeOption("westerlander_education_goldsmith_option", new TextObject("{=bab_westerlander_education_goldsmith_title}trained with the metalworkers of the west."), new TextObject("{=bab_westerlander_education_goldsmith_desc}You learned to judge ore and alloys and watched skilled hands turn the wealth of the hills into tools, fittings, bright plate, and delicate goldwork."), (args, u) => u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsWesterlander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.ArtisanUrban, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_grit", "", ""));
        yield return new NarrativeOption("westerlander_education_rock_court_option", new TextObject("{=bab_westerlander_education_rock_court_title}were fostered in a wealthy court."), new TextObject("{=bab_westerlander_education_rock_court_desc}You entered the household of a powerful lord, where strict ceremony, careful accounts, and the ambitions of richly armed retainers taught you how power was displayed and preserved."), (args, u) => u.UpgradeLeadershipCharmSkills(args, DefaultCharacterAttributes.Social), ccm => IsWesterlander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("westerlander_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_westerlander_youth_groom_desc}You cared for a wealthy knight's horses and observed the strict order of a great Westerlands household."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsWesterlander, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("westerlander_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_westerlander_youth_cavalry_desc}You trained with well-equipped cavalry, learning to trust in strong armour, close formation, and a decisive charge."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsWesterlander, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("westerlander_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_westerlander_youth_guard_desc}You guarded a mine, town, or hilltop castle where gold and iron demanded constant vigilance."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsWesterlander, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("westerlander_youth_bandit_option", new TextObject("{=bab_westerlander_youth_bandit_title}ran with an outlaw band."), new TextObject("{=bab_westerlander_youth_bandit_desc}You joined outlaws who haunted mining roads and wooded hills, robbing rich traffic while evading a lord's patrols."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsWesterlander, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("westerlander_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_westerlander_youth_infantry_desc}You drilled with disciplined men-at-arms equipped from the wealth of the Westerlands' mines and armouries."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsWesterlander, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("westerlander_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_westerlander_youth_skirmisher_desc}You learned to scout rocky hills and loose missiles into enemy ranks before their heavier troops could close."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsWesterlander, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsWesterlander(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "westerlander";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsWesterlander, ccm =>
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

    private const string CultureIdValue = "westerlander";
}
