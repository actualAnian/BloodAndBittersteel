using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

/// <summary>
/// Shared implementation for the regional Westerosi cultures.  The visible text is
/// supplied by each concrete culture so every original line has its own BAB key,
/// while vanilla strings retain their original localization keys.
/// </summary>
public abstract class WesterosiCultureBase : ICharacterCreationCulture
{
    protected abstract CultureProfile Profile { get; }
    public string CultureId => Profile.Id;

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return Parent("retainer", new TextObject("{=bab_crownlander_parent_retainer_title}A lord's retainers"), Profile.ParentRetainer, (a, u) => u.UpgradeRidingPolearmSkills(a, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return Parent("merchant", new TextObject("{=651FhzdR}Urban merchants"), Profile.ParentMerchant, (a, u) => u.UpgradeTradeCharmSkills(a, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return Parent("farmer", new TextObject("{=RDfXuVxT}Yeomen"), Profile.ParentFarmer, (a, u) => u.UpgradePolearmCrossbowSkills(a, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return Parent("blacksmith", new TextObject("{=p2KIhGbE}Urban blacksmith"), Profile.ParentBlacksmith, (a, u) => u.UpgradeCraftingTwoHandedSkills(a, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return Parent("hunter", new TextObject("{=YcnK0Thk}Hunters"), Profile.ParentHunter, (a, u) => u.UpgradeScoutingCrossbowSkills(a, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;
    public IEnumerable<NarrativeOption> GetEducationOptions() => MainlandEducationOptions.Shared;

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return Youth("groom", new TextObject("{=bab_crownlander_youth_groom_title}served as a lord's groom."), Profile.YouthGroom, (a, u) => u.UpgradeCharmTacticsSkills(a, DefaultCharacterAttributes.Social), "noble", "act_childhood_sharp");
        yield return Youth("cavalry", new TextObject("{=h2KnarLL}trained with the cavalry."), Profile.YouthCavalry, (a, u) => u.UpgradeRidingPolearmSkills(a, DefaultCharacterAttributes.Endurance), "cavalry", "act_childhood_apprentice");
        yield return Youth("guard", new TextObject("{=aTncHUfL}stood guard with the garrisons."), Profile.YouthGuard, (a, u) => u.UpgradeCrossbowEngineeringSkills(a, DefaultCharacterAttributes.Intelligence), "guard", "act_childhood_vibrant");
        yield return Youth("bandit", Profile.BanditTitle, Profile.YouthBandit, (a, u) => u.UpgradeRogueryThrowingSkills(a, DefaultCharacterAttributes.Cunning), "bandit", "act_childhood_militia");
        yield return Youth("infantry", new TextObject("{=a8arFSra}trained with the infantry."), Profile.YouthInfantry, (a, u) => u.UpgradePolearmOneHandedSkills(a, DefaultCharacterAttributes.Vigor), "infantry", "act_childhood_fierce");
        yield return Youth("skirmisher", new TextObject("{=oMbOIPc9}joined the skirmishers."), Profile.YouthSkirmisher, (a, u) => u.UpgradeThrowingOneHandedSkills(a, DefaultCharacterAttributes.Control), "skirmisher", "act_childhood_fox");
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

    private bool IsSelected(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == CultureId;

    private NarrativeOption Parent(string option, TextObject title, TextObject description, Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> upgrades, string occupation, string motherAnimation, string fatherAnimation)
    {
        return new NarrativeOption(CultureId + "_" + option + "_option", title, description, upgrades, IsSelected, ccm =>
        {
            ccm.CharacterCreationContent.SetParentOccupation(occupation);
            string mother = NarrativeEquipmentHelper.GetMotherEquipmentId(ccm, occupation, CultureId);
            string father = NarrativeEquipmentHelper.GetFatherEquipmentId(ccm, occupation, CultureId);
            NarrativeEquipmentHelper.SetParentEquipment(ccm, mother, father, motherAnimation, fatherAnimation);
        });
    }

    private NarrativeOption Youth(string option, TextObject title, TextObject description, Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> upgrades, string equipmentType, string animation)
    {
        return new NarrativeOption(CultureId + "_youth_" + option + "_option", title, description, upgrades, IsSelected, ccm =>
        {
            ccm.CharacterCreationContent.SelectedTitleType = equipmentType;
            string equipment = NarrativeEquipmentHelper.GetPlayerEquipmentId(ccm, equipmentType, CultureId, Hero.MainHero.IsFemale);
            NarrativeEquipmentHelper.SetPlayerEquipment(ccm, equipment, animation);
        });
    }
}

public sealed class CultureProfile
{
    public string Id { get; }
    public TextObject ParentRetainer { get; }
    public TextObject ParentMerchant { get; }
    public TextObject ParentFarmer { get; }
    public TextObject ParentBlacksmith { get; }
    public TextObject ParentHunter { get; }
    public TextObject YouthGroom { get; }
    public TextObject YouthCavalry { get; }
    public TextObject YouthGuard { get; }
    public TextObject BanditTitle { get; }
    public TextObject YouthBandit { get; }
    public TextObject YouthInfantry { get; }
    public TextObject YouthSkirmisher { get; }

    public CultureProfile(string id, string parentRetainer, string parentMerchant, string parentFarmer, string parentBlacksmith, string parentHunter, string youthGroom, string youthCavalry, string youthGuard, string banditTitle, string youthBandit, string youthInfantry, string youthSkirmisher)
    {
        Id = id;
        ParentRetainer = new TextObject(parentRetainer);
        ParentMerchant = new TextObject(parentMerchant);
        ParentFarmer = new TextObject(parentFarmer);
        ParentBlacksmith = new TextObject(parentBlacksmith);
        ParentHunter = new TextObject(parentHunter);
        YouthGroom = new TextObject(youthGroom);
        YouthCavalry = new TextObject(youthCavalry);
        YouthGuard = new TextObject(youthGuard);
        BanditTitle = new TextObject(banditTitle);
        YouthBandit = new TextObject(youthBandit);
        YouthInfantry = new TextObject(youthInfantry);
        YouthSkirmisher = new TextObject(youthSkirmisher);
    }
}
