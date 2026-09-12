using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public class VlandianCulture : ICharacterCreationCulture
{
    public string CultureId => "vlandia";

    public IEnumerable<NarrativeOption> GetParentOptions()
    {
        yield return CreateParentOption("vlandia_retainer_option", "{=2TptWc4m}A baron's retainers", "{=0Suu1Q9q}Your father was a bailiff for a local feudal magnate. He looked after his liege's estates, resolved disputes in the village, and helped train the village levy. He rode with the lord's cavalry, fighting as an armored knight.", (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Social), CharacterOccupations.Retainer, "act_character_creation_female_default_side_to_side_1", "act_character_creation_male_default_side_to_side_1");
        yield return CreateParentOption("vlandia_merchant_option", "{=651FhzdR}Urban merchants", "{=qNZFkxJb}Your family were merchants in one of the main cities of the kingdom. They organized caravans to nearby towns and were active in the local merchant's guild.", (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Intelligence), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_mother_front", "act_character_creation_male_default_mother_front");
        yield return CreateParentOption("vlandia_farmer_option", "{=RDfXuVxT}Yeomen", "{=BLZ4mdhb}Your family were small farmers with just enough land to feed themselves and make a small profit. People like them were the pillars of the kingdom's economy, as well as the backbone of the levy.", (args, u) => u.UpgradePolearmCrossbowSkills(args, DefaultCharacterAttributes.Endurance), CharacterOccupations.Farmer, "act_character_creation_female_default_father_sitting", "act_character_creation_male_default_father_sitting");
        yield return CreateParentOption("vlandia_blacksmith_option", "{=p2KIhGbE}Urban blacksmith", "{=btsMpRcA}Your family owned a smithy in a city. Your father played an active if minor role in the town council, and also served in the militia.", (args, u) => u.UpgradeCraftingTwoHandedSkills(args, DefaultCharacterAttributes.Vigor), CharacterOccupations.ArtisanUrban, "act_character_creation_female_default_side_to_side_2", "act_character_creation_male_default_side_to_side_2");
        yield return CreateParentOption("vlandia_hunter_option", "{=YcnK0Thk}Hunters", "{=yRFSzSDZ}Your family lived in a village, but did not own their own land. Instead, your father supplemented paid jobs with long trips in the woods, hunting and trapping, always keeping a wary eye for the lord's game wardens.", (args, u) => u.UpgradeScoutingCrossbowSkills(args, DefaultCharacterAttributes.Control), CharacterOccupations.Hunter, "act_character_creation_female_default_side_to_side_3", "act_character_creation_male_default_side_to_side_3");
        yield return CreateParentOption("vlandia_mercenary_option", "{=ipQP6aVi}Mercenaries", "{=yYhX6JQC}Your father joined one of Vlandia's many mercenary companies, composed of men who got such a taste for war in their lord's service that they never took well to peace. Their crossbowmen were much valued across Calradia. Your mother was a camp follower, taking you along in the wake of bloody campaigns.", (args, u) => u.UpgradeRogueryCrossbowSkills(args, DefaultCharacterAttributes.Cunning), CharacterOccupations.MerchantUrban, "act_character_creation_female_default_hugging", "act_character_creation_male_default_hugging");
    }

    public IEnumerable<NarrativeOption> GetChildhoodOptions() => MainlandChildhoodOptions.Shared;

    public IEnumerable<NarrativeOption> GetEducationOptions() => MainlandEducationOptions.Shared;

    public IEnumerable<NarrativeOption> GetYouthOptions()
    {
        yield return new NarrativeOption("youth_groom_option", new TextObject("{=bhE2i6OU}served as a baron's groom."), new TextObject("{=i3k7YtA8}You were chosen by a knight to accompany a minor baron of the Vlandian kingdom. You were not given major responsibilities - mostly carrying messages and tending to his horse - but it did give you a chance to see how campaigns were planned and men were deployed in battle."), (args, u) => u.UpgradeCharmTacticsSkills(args, DefaultCharacterAttributes.Social), IsVlandia, ccm => SelectYouth(ccm, "retainer", "act_childhood_sharp"));
        yield return new NarrativeOption("youth_cavalry_option", new TextObject("{=h2KnarLL}trained with the cavalry."), new TextObject("{=7cHsIMLP}You could never have bought the equipment on your own, but you were a good enough rider so that the local lord lent you a horse and equipment. You joined the armored cavalry, training with the lance."), (args, u) => u.UpgradeRidingPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsVlandia, ccm => SelectYouth(ccm, "mercenary", "act_childhood_apprentice"));
        yield return new NarrativeOption("youth_guard_high_register_option", new TextObject("{=aTncHUfL}stood guard with the garrisons."), new TextObject("{=63TAYbkx}Urban troops spend much of their time guarding the town walls. Most of their training was in missile weapons, especially useful during sieges."), (args, u) => u.UpgradeCrossbowEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsVlandia, ccm => SelectYouth(ccm, "guard", "act_childhood_vibrant"));
        yield return new NarrativeOption("youth_camp_option", new TextObject("{=GFUggps8}marched with the camp followers."), new TextObject("{=64rWqBLN}You avoided service with one of the main forces of your realm's armies, but followed instead in the train - the troops' wives, lovers and servants, and those who make their living by caring for, entertaining, or cheating the soldiery."), (args, u) => u.UpgradeRogueryThrowingSkills(args, DefaultCharacterAttributes.Cunning), IsVlandia, ccm => SelectYouth(ccm, "bard", "act_childhood_militia"));
        yield return new NarrativeOption("youth_infantry_option", new TextObject("{=a8arFSra}trained with the infantry."), new TextObject("{=afH90aNs}Levy armed with spear and shield, drawn from smallholding farmers, have always been the backbone of most armies of Calradia."), (args, u) => u.UpgradePolearmOneHandedSkills(args, DefaultCharacterAttributes.Vigor), IsVlandia, ccm => SelectYouth(ccm, "infantry", "act_childhood_fierce"));
        yield return new NarrativeOption("youth_skirmisher_option", new TextObject("{=oMbOIPc9}joined the skirmishers."), new TextObject("{=bXAg5w19}Younger recruits, or those of a slighter build, or those too poor to buy shield and armor tend to join the skirmishers. Fighting with bow and javelin, they try to stay out of reach of the main enemy forces."), (args, u) => u.UpgradeThrowingOneHandedSkills(args, DefaultCharacterAttributes.Control), IsVlandia, ccm => SelectYouth(ccm, "skirmisher", "act_childhood_fox"));
    }

    public IEnumerable<NarrativeOption> GetAdulthoodOptions()
    {
        yield return new NarrativeOption("adulthood_defeated_enemy_option", new TextObject("{=8bwpVpgy}you defeated an enemy in battle."), new TextObject("{=1IEroJKs}Not everyone who musters for the levy marches to war, and not everyone who goes on campaign sees action. You did both, and you also took down an enemy warrior in direct one-to-one combat, in the full view of your comrades."), (args, u) =>
        {
            u.UpgradeOneHandedTwoHandedSkills(args, DefaultCharacterAttributes.Vigor);
            args.SetAffectedTraits(new[] { DefaultTraits.Valor });
            args.SetLevelToTraits(1);
            args.SetRenownToAdd(20);
        }, _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_athlete"));

        yield return AdulthoodCultureOptions.Manhunt;
        yield return AdulthoodCultureOptions.CaravanLeader;

        yield return new NarrativeOption("adulthood_workshop_option", new TextObject("{=xORjDTal}you invested some money in a workshop."), new TextObject("{=PyVqDLBu}Your parents didn't give you much money, but they did leave just enough for you to secure a loan against a larger amount to build a small workshop. You paid back what you borrowed, and sold your enterprise for a profit."), (args, u) =>
        {
            u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence);
            args.SetAffectedTraits(new[] { DefaultTraits.Calculating });
            args.SetLevelToTraits(1);
            args.SetRenownToAdd(10);
        }, IsUrban, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_decisive"));

        yield return new NarrativeOption("adulthood_investor_option", new TextObject("{=xKXcqRJI}you invested some money in land."), new TextObject("{=cbF9jdQo}Your parents didn't give you much money, but they did leave just enough for you to purchase a plot of unused land at the edge of the village. You cleared away rocks and dug an irrigation ditch, raised a few seasons of crops, than sold it for a considerable profit."), (args, u) =>
        {
            u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence);
            args.SetAffectedTraits(new[] { DefaultTraits.Calculating });
            args.SetLevelToTraits(1);
            args.SetRenownToAdd(10);
        }, IsRural, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_decisive"));

        yield return new NarrativeOption("adulthood_hunter_option", new TextObject("{=TbNRtUjb}you hunted a dangerous animal."), new TextObject("{=I3PcdaaL}Wolves, bears are a constant menace to the flocks of northern Calradia, while hyenas and leopards trouble the south. You went with a group of your fellow villagers and fired the missile that brought down the beast."), (args, u) =>
        {
            u.UpgradePolearmAthleticsSkills(args, DefaultCharacterAttributes.Control);
            args.SetAffectedTraits(new[] { DefaultTraits.Valor });
            args.SetLevelToTraits(1);
            args.SetRenownToAdd(5);
        }, IsRural, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_tough"));

        yield return new NarrativeOption("adulthood_siege_survivor_option", new TextObject("{=WbHfGCbd}you survived a siege."), new TextObject("{=FhZPjhli}Your hometown was briefly placed under siege, and you were called to defend the walls. Everyone did their part to repulse the enemy assault, and everyone is justly proud of what they endured."), (args, u) =>
        {
            u.UpgradeBowCrossbowSkills(args, DefaultCharacterAttributes.Control);
            args.SetRenownToAdd(5);
        }, IsUrban, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_tough"));

        yield return new NarrativeOption("adulthood_escapade_high_register_option", new TextObject("{=kNXet6Um}you had a famous escapade in town."), new TextObject("{=DjeAJtix}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, on one of your trips into town you got into the kind of trouble from which only a quick tongue or quick feet get you out alive."), (args, u) =>
        {
            u.UpgradeAthleticsRoguerySkills(args, DefaultCharacterAttributes.Endurance);
            args.SetAffectedTraits(new[] { DefaultTraits.Valor });
            args.SetLevelToTraits(1);
            args.SetRenownToAdd(5);
        }, IsRural, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_clever"));

        yield return new NarrativeOption("adulthood_escapade_low_register_option", new TextObject("{=qlOuiKXj}you had a famous escapade."), new TextObject("{=lD5Ob3R4}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, you got into the kind of trouble from which only a quick tongue or quick feet get you out alive."), (args, u) =>
        {
            u.UpgradeAthleticsRoguerySkills(args, DefaultCharacterAttributes.Endurance);
            args.SetAffectedTraits(new[] { DefaultTraits.Valor });
            args.SetLevelToTraits(1);
            args.SetRenownToAdd(5);
        }, IsUrban, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_clever"));

        yield return new NarrativeOption("adulthood_nice_person_option", new TextObject("{=Yqm0Dics}you treated people well."), new TextObject("{=dDmcqTzb}Yours wasn't the kind of reputation that local legends are made of, but it was the kind that wins you respect among those around you. You were consistently fair and honest in your business dealings and helpful to those in trouble. In doing so, you got a sense of what made people tick."), (args, u) =>
        {
            u.UpgradeStewardCharmSkills(args, DefaultCharacterAttributes.Social);
            args.SetAffectedTraits(new[] { DefaultTraits.Mercy, DefaultTraits.Generosity, DefaultTraits.Honor });
            args.SetLevelToTraits(1);
            args.SetRenownToAdd(5);
        }, _ => true, ccm => NarrativeEquipmentHelper.SetPlayerCharacterAnimation(ccm, "player_adulthood_character", "act_childhood_manners"));
    }

    private static bool IsVlandia(CharacterCreationManager ccm) => ccm.CharacterCreationContent.SelectedCulture.StringId == "vlandia";
    private static bool IsUrban(CharacterCreationManager ccm) => CharacterOccupations.IsUrbanOccupation(ccm.CharacterCreationContent.SelectedParentOccupation);
    private static bool IsRural(CharacterCreationManager ccm) => !CharacterOccupations.IsUrbanOccupation(ccm.CharacterCreationContent.SelectedParentOccupation);

    private static NarrativeOption CreateParentOption(string id, string titleKey, string descKey, System.Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, string occupationType, string motherAnim, string fatherAnim)
    {
        return new NarrativeOption(id, new TextObject(titleKey), new TextObject(descKey), setArgs, IsVlandia, ccm =>
        {
            ccm.CharacterCreationContent.SetParentOccupation(occupationType);
            string cultureId = ccm.CharacterCreationContent.SelectedCulture.StringId;
            string motherEquipmentId = NarrativeEquipmentHelper.GetMotherEquipmentId(ccm, ccm.CharacterCreationContent.SelectedParentOccupation, cultureId);
            string fatherEquipmentId = NarrativeEquipmentHelper.GetFatherEquipmentId(ccm, ccm.CharacterCreationContent.SelectedParentOccupation, cultureId);
            NarrativeEquipmentHelper.SetParentEquipment(ccm, motherEquipmentId, fatherEquipmentId, motherAnim, fatherAnim);
        });
    }

    private static void SelectYouth(CharacterCreationManager ccm, string titleType, string animation)
    {
        ccm.CharacterCreationContent.SelectedTitleType = titleType;
        string playerEquipmentId = NarrativeEquipmentHelper.GetPlayerEquipmentId(ccm, ccm.CharacterCreationContent.SelectedTitleType, ccm.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
        NarrativeEquipmentHelper.SetPlayerEquipment(ccm, playerEquipmentId, animation);
    }
}
