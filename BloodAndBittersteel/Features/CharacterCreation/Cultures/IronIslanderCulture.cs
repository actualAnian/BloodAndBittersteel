using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class IronIslanderCulture : ICharacterCreationCulture
{
    public string CultureId => "ironislander";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("ironislander_retainer_option", "{=bab_crownlander_parent_retainer_title}A lord's retainers", "{=bab_ironislander_parent_retainer_desc}Your family served an Ironborn lord or captain, overseeing his hall, ships, and fighting men when the banners were raised.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("ironislander_merchant_option", "{=651FhzdR}Urban merchants", "{=bab_ironislander_parent_merchant_desc}Your family traded fish, iron, timber, and goods brought home by the longships of the Iron Islands.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("ironislander_farmer_option", "{=RDfXuVxT}Yeomen", "{=bab_ironislander_parent_farmer_desc}Your family scratched a living from thin soil and cold shores, working land and sea under a local lord.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("ironislander_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=bab_ironislander_parent_blacksmith_desc}Your family worked an island forge, shaping local iron into tools, axes, mail, and fittings for ships.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("ironislander_hunter_option", "{=YcnK0Thk}Hunters", "{=bab_ironislander_parent_hunter_desc}Your family hunted seabirds and seals along the coasts and knew every cove, cliff path, and treacherous current nearby.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions()
    {
        foreach (NarrativeOption option in MainlandEducationOptions.Shared)
            yield return option;

        yield return new NarrativeOption("ironislander_education_joined_crew_option", new TextObject("{=bab_ironislander_education_joined_crew_title}joined a longship's crew."), new TextObject("{=bab_ironislander_education_joined_crew_desc}At the age when other children entered apprenticeships, you went aboard a longship and learned the oar, sail, axe, and harsh discipline of a captain's crew."), (args, u) => u.UpgradeAthleticsOneHandedSkills(args, DefaultCharacterAttributes.Vigor), ccm => IsIronIslander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer && !CharacterObject.PlayerCharacter.IsFemale, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_athlete", "", ""));
        yield return new NarrativeOption("ironislander_education_captains_hall_option", new TextObject("{=bab_ironislander_education_captains_hall_title}helped govern a captain's hall."), new TextObject("{=bab_ironislander_education_captains_hall_desc}You learned to reckon stores, direct servants and thralls, receive petitioners, and keep a hard household functioning while its fighting men were away at sea."), (args, u) => u.UpgradeStewardTacticsSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsIronIslander(ccm) && ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.Retainer && CharacterObject.PlayerCharacter.IsFemale, ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_manners", "", ""));
        yield return new NarrativeOption("ironislander_education_shipwright_option", new TextObject("{=bab_ironislander_education_shipwright_title}worked beside shipwrights and sailmakers."), new TextObject("{=bab_ironislander_education_shipwright_desc}You carried timber, pitch, rope, and cloth while learning how island craftsmen built and supplied the longships upon which every captain's fortune depended."), (args, u) => u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence), ccm => IsIronIslander(ccm) && (ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.MerchantUrban || ccm.CharacterCreationContent.SelectedParentOccupation == CharacterOccupations.ArtisanUrban), ccm => MainlandEducationOptions.SetEducationAnim(ccm, "act_childhood_grit", "", ""));
    }

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("ironislander_youth_groom_option", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), new TextObject("{=bab_ironislander_youth_groom_desc}You served in a captain's household, tending his few valuable horses while learning the harder duties expected aboard ship."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsIronIslander, ccm => SelectYouth(ccm, "noble", "act_childhood_sharp"));
        yield return new NarrativeOption("ironislander_youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=bab_ironislander_youth_cavalry_desc}Though horses were scarce on the islands, you trained as one of the few mounted retainers maintained by a wealthy lord."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsIronIslander, ccm => SelectYouth(ccm, "cavalry", "act_childhood_apprentice"));
        yield return new NarrativeOption("ironislander_youth_guard_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=bab_ironislander_youth_guard_desc}You stood watch over a harbour and sea-beaten keep, learning to spot unfamiliar sails beyond the headlands."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsIronIslander, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("ironislander_youth_bandit_option", new TextObject("{=bab_ironislander_youth_bandit_title}sailed with reavers."), new TextObject("{=bab_ironislander_youth_bandit_desc}You went to sea with a hard captain, learning boarding, coastal raiding, and how to vanish beyond the horizon with your spoils."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsIronIslander, ccm => SelectYouth(ccm, "bandit", "act_childhood_militia"));
        yield return new NarrativeOption("ironislander_youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=bab_ironislander_youth_infantry_desc}You trained to fight on deck and ashore with axe, spear, and shield in the close press of a landing."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsIronIslander, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("ironislander_youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bab_ironislander_youth_skirmisher_desc}You learned to cast missiles from a rolling deck and screen a raiding party as it withdrew toward the ships."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsIronIslander, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
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

    private static bool IsIronIslander(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "ironislander";

    private static NarrativeOption CreateParentOption(string id, string titleText, string descriptionText, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(id, new TextObject(titleText), new TextObject(descriptionText), setArgs, IsIronIslander, ccm =>
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

    private const string CultureIdValue = "ironislander";
}
