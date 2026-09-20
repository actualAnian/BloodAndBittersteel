using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation;

public static class AdulthoodCultureOptions
{
    private static bool IsRural(CharacterCreationManager ccm) => !CharacterOccupations.IsUrbanOccupation(ccm.CharacterCreationContent.SelectedParentOccupation);
    private static bool IsUrban(CharacterCreationManager ccm) => CharacterOccupations.IsUrbanOccupation(ccm.CharacterCreationContent.SelectedParentOccupation);

    private static readonly string[] ManhuntCultures = { "vlandia", "crownlander" };
    private static readonly string[] CaravanLeaderCultures = { "vlandia", "crownlander" };
    private static readonly string[] SavedVillageCultures = { "sturgia" };

    private static bool IsManhuntAvailable(CharacterCreationManager ccm)
    {
        string culture = ccm.CharacterCreationContent.SelectedCulture.StringId;
        return IsRural(ccm) && ManhuntCultures.Contains(culture);
    }

    private static bool IsCaravanLeaderAvailable(CharacterCreationManager ccm)
    {
        string culture = ccm.CharacterCreationContent.SelectedCulture.StringId;
        return IsUrban(ccm) && CaravanLeaderCultures.Contains(culture);
    }

    private static bool IsSavedVillageAvailable(CharacterCreationManager ccm)
    {
        string culture = ccm.CharacterCreationContent.SelectedCulture.StringId;
        return IsRural(ccm) && SavedVillageCultures.Contains(culture);
    }

    public static NarrativeOption Manhunt => new("adulthood_manhunt_option", new TextObject("{=mP3uFbcq}you led a successful manhunt."), new TextObject("{=4f5xwzX0}When your community needed to organize a posse to pursue horse thieves, you were the obvious choice. You hunted down the raiders, surrounded them and forced their surrender, and took back your stolen property."), (args, u) =>
    {
        u.UpgradeStewardTacticsSkills(args, DefaultCharacterAttributes.Cunning);
        args.SetAffectedTraits(new[] { DefaultTraits.Calculating });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(10);
    }, IsManhuntAvailable, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_battania_mp_clan_warrior_shieldperk_idle"));

    public static NarrativeOption CaravanLeader => new("adulthood_caravan_leader_option", new TextObject("{=wfbtS71d}you led a caravan."), new TextObject("{=joRHKCkm}Your family needed someone trustworthy to take a caravan to a neighboring town. You organized supplies, ensured a constant watch to keep away bandits, and brought it safely to its destination."), (args, u) =>
    {
        u.UpgradeTradeLeadershipSkills(args, DefaultCharacterAttributes.Cunning);
        args.SetAffectedTraits(new[] { DefaultTraits.Calculating });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(10);
    }, IsCaravanLeaderAvailable, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_ready_handshield"));

    public static NarrativeOption SavedVillage => new("adulthood_saved_village_option", new TextObject("{=x1HTX5hq}you saved your village from a flood."), new TextObject("{=bWlmGDf3}When a sudden storm caused the local stream to rise suddenly, your neighbors needed quick-thinking leadership. You provided it, directing them to build levees to save their homes."), (args, u) =>
    {
        u.UpgradeStewardTacticsSkills(args, DefaultCharacterAttributes.Cunning);
        args.SetAffectedTraits(new[] { DefaultTraits.Valor });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(10);
    }, IsSavedVillageAvailable, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_drafted_to_war_pose"));

    public static NarrativeOption DefeatedEnemy => new("adulthood_defeated_enemy_option", new TextObject("{=8bwpVpgy}you defeated an enemy in battle."), new TextObject("{=1IEroJKs}Not everyone who musters for the levy marches to war, and not everyone who goes on campaign sees action. You did both, and you also took down an enemy warrior in direct one-to-one combat, in the full view of your comrades."), (args, u) =>
    {
        u.UpgradeOneHandedTwoHandedSkills(args, DefaultCharacterAttributes.Vigor);
        args.SetAffectedTraits(new[] { DefaultTraits.Valor });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(20);
    }, _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_athlete"));

    public static NarrativeOption Workshop => new("adulthood_workshop_option", new TextObject("{=xORjDTal}you invested some money in a workshop."), new TextObject("{=PyVqDLBu}Your parents didn't give you much money, but they did leave just enough for you to secure a loan against a larger amount to build a small workshop. You paid back what you borrowed, and sold your enterprise for a profit."), (args, u) =>
    {
        u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence);
        args.SetAffectedTraits(new[] { DefaultTraits.Calculating });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(10);
    }, IsUrban, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_decisive"));

    public static NarrativeOption Investor => new("adulthood_investor_option", new TextObject("{=xKXcqRJI}you invested some money in land."), new TextObject("{=cbF9jdQo}Your parents didn't give you much money, but they did leave just enough for you to purchase a plot of unused land at the edge of the village. You cleared away rocks and dug an irrigation ditch, raised a few seasons of crops, than sold it for a considerable profit."), (args, u) =>
    {
        u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence);
        args.SetAffectedTraits(new[] { DefaultTraits.Calculating });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(10);
    }, IsRural, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_decisive"));

    public static NarrativeOption Hunter => new("adulthood_hunter_option", new TextObject("{=TbNRtUjb}you hunted a dangerous animal."), new TextObject("{=I3PcdaaL}Wolves, bears are a constant menace to the flocks of northern Calradia, while hyenas and leopards trouble the south. You went with a group of your fellow villagers and fired the missile that brought down the beast."), (args, u) =>
    {
        u.UpgradePolearmAthleticsSkills(args, DefaultCharacterAttributes.Control);
        args.SetAffectedTraits(new[] { DefaultTraits.Valor });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(5);
    }, IsRural, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_tough"));

    public static NarrativeOption SiegeSurvivor => new("adulthood_siege_survivor_option", new TextObject("{=WbHfGCbd}you survived a siege."), new TextObject("{=FhZPjhli}Your hometown was briefly placed under siege, and you were called to defend the walls. Everyone did their part to repulse the enemy assault, and everyone is justly proud of what they endured."), (args, u) =>
    {
        u.UpgradeBowCrossbowSkills(args, DefaultCharacterAttributes.Control);
        args.SetRenownToAdd(5);
    }, IsUrban, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_tough"));

    public static NarrativeOption EscapadeHigh => new("adulthood_escapade_high_register_option", new TextObject("{=kNXet6Um}you had a famous escapade in town."), new TextObject("{=DjeAJtix}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, on one of your trips into town you got into the kind of trouble from which only a quick tongue or quick feet get you out alive."), (args, u) =>
    {
        u.UpgradeAthleticsRoguerySkills(args, DefaultCharacterAttributes.Endurance);
        args.SetAffectedTraits(new[] { DefaultTraits.Valor });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(5);
    }, IsRural, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_clever"));

    public static NarrativeOption EscapadeLow => new("adulthood_escapade_low_register_option", new TextObject("{=qlOuiKXj}you had a famous escapade."), new TextObject("{=lD5Ob3R4}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, you got into the kind of trouble from which only a quick tongue or quick feet get you out alive."), (args, u) =>
    {
        u.UpgradeAthleticsRoguerySkills(args, DefaultCharacterAttributes.Endurance);
        args.SetAffectedTraits(new[] { DefaultTraits.Valor });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(5);
    }, IsUrban, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_clever"));

    public static NarrativeOption NicePerson => new("adulthood_nice_person_option", new TextObject("{=Yqm0Dics}you treated people well."), new TextObject("{=dDmcqTzb}Yours wasn't the kind of reputation that local legends are made of, but it was the kind that wins you respect among those around you. You were consistently fair and honest in your business dealings and helpful to those in trouble. In doing so, you got a sense of what made people tick."), (args, u) =>
    {
        u.UpgradeStewardCharmSkills(args, DefaultCharacterAttributes.Social);
        args.SetAffectedTraits(new[] { DefaultTraits.Mercy, DefaultTraits.Generosity, DefaultTraits.Honor });
        args.SetLevelToTraits(1);
        args.SetRenownToAdd(5);
    }, _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_manners"));
}
