using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation;

public static class MainlandEducationOptions
{
    private static bool HasOccupation(CharacterCreationManager ccm, string occupation) => ccm.CharacterCreationContent.SelectedParentOccupation == occupation;
    private static bool IsRetainer(CharacterCreationManager ccm) => HasOccupation(ccm, CharacterOccupations.Retainer);
    private static bool IsMaleRetainer(CharacterCreationManager ccm) => IsRetainer(ccm) && !CharacterObject.PlayerCharacter.IsFemale;
    private static bool IsFemaleRetainer(CharacterCreationManager ccm) => IsRetainer(ccm) && CharacterObject.PlayerCharacter.IsFemale;
    private static bool IsMerchant(CharacterCreationManager ccm) => HasOccupation(ccm, CharacterOccupations.MerchantUrban);
    private static bool IsArtisan(CharacterCreationManager ccm) => HasOccupation(ccm, CharacterOccupations.ArtisanUrban);
    private static bool IsFarmer(CharacterCreationManager ccm) => HasOccupation(ccm, CharacterOccupations.Farmer);
    private static bool IsHunter(CharacterCreationManager ccm) => HasOccupation(ccm, CharacterOccupations.Hunter);

    public static readonly IReadOnlyList<NarrativeOption> Shared = new NarrativeOption[]
    {
        new NarrativeOption("education_retainer_page_option", new TextObject("{=bab_education_retainer_page_title}served as a page in a noble household."), new TextObject("{=bab_education_retainer_page_desc}You were placed in a noble household, where you waited at table, carried messages, cared for arms and horses, and learned the duties expected of a future retainer."), (args, u) => u.UpgradeLeadershipTacticsSkills(args, DefaultCharacterAttributes.Social), IsMaleRetainer, ccm => SetEducationAnim(ccm, "act_childhood_manners", "", "")),
        new NarrativeOption("education_retainer_lady_option", new TextObject("{=bab_education_retainer_lady_title}attended a noble lady."), new TextObject("{=bab_education_retainer_lady_desc}You entered a great household as a companion and attendant, learning courtly conduct, correspondence, household management, and how to navigate the rivalries of a lord's hall."), (args, u) => u.UpgradeStewardCharmSkills(args, DefaultCharacterAttributes.Social), IsFemaleRetainer, ccm => SetEducationAnim(ccm, "act_childhood_manners", "", "")),
        new NarrativeOption("education_retainer_tutor_option", new TextObject("{=EMVojYzW}studied with your private tutor."), new TextObject("{=bab_education_retainer_tutor_desc}Your family's position gave you lessons in letters, sums, histories, and the laws and obligations that bound lord, household, and tenant together."), (args, u) => u.UpgradeEngineeringLeadershipSkills(args, DefaultCharacterAttributes.Intelligence), IsRetainer, ccm => SetEducationAnim(ccm, "act_childhood_book", "character_creation_notebook", "")),
        new NarrativeOption("education_retainer_stables_option", new TextObject("{=hin3iA2D}cared for the horses."), new TextObject("{=bab_education_retainer_stables_desc}You spent long days in the household stables, learning to feed, groom, exercise, and account for the valuable horses kept by your lord."), (args, u) => u.UpgradeRidingStewardSkills(args, DefaultCharacterAttributes.Endurance), IsRetainer, ccm => SetEducationAnim(ccm, "act_childhood_peddlers_2", "", "_to_carry_bd_fabric_c")),
        new NarrativeOption("education_merchant_accounts_option", new TextObject("{=bab_education_merchant_accounts_title}kept the family accounts."), new TextObject("{=bab_education_merchant_accounts_desc}You learned letters, sums, weights, measures, contracts, and the careful keeping of ledgers for your family's trade."), (args, u) => u.UpgradeEngineeringTradeSkills(args, DefaultCharacterAttributes.Intelligence), IsMerchant, ccm => SetEducationAnim(ccm, "act_childhood_book", "character_creation_notebook", "")),
        new NarrativeOption("education_merchant_market_option", new TextObject("{=JTsv6PFe}worked in the markets and caravanserais."), new TextObject("{=bab_education_merchant_market_desc}You bargained with suppliers, inspected wares, and learned which words opened purses and which promises could safely be trusted."), (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Social), IsMerchant, ccm => SetEducationAnim(ccm, "act_childhood_manners", "", "")),
        new NarrativeOption("education_merchant_caravan_option", new TextObject("{=bab_education_merchant_caravan_title}helped organize trading journeys."), new TextObject("{=bab_education_merchant_caravan_desc}You helped arrange guards, pack animals, provisions, tolls, and letters of passage for merchants travelling between distant markets."), (args, u) => u.UpgradeTradeLeadershipSkills(args, DefaultCharacterAttributes.Social), IsMerchant, ccm => SetEducationAnim(ccm, "act_childhood_peddlers_2", "", "_to_carry_bd_fabric_c")),
        new NarrativeOption("education_artisan_workshop_option", new TextObject("{=bab_education_artisan_workshop_title}learned the family craft."), new TextObject("{=bab_education_artisan_workshop_desc}You began with sweeping floors and carrying fuel before your family trusted you with tools, materials, customers, and the secrets of their craft."), (args, u) => u.UpgradeTradeCraftingSkills(args, DefaultCharacterAttributes.Intelligence), IsArtisan, ccm => SetEducationAnim(ccm, "act_childhood_militia", "", "peasant_hammer_1_t1")),
        new NarrativeOption("education_artisan_master_option", new TextObject("{=bab_education_artisan_master_title}apprenticed under another master."), new TextObject("{=bab_education_artisan_master_desc}Your family placed you in another master's household, where strict instruction and long labour taught you new methods and professional discipline."), (args, u) => u.UpgradeCraftingEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsArtisan, ccm => SetEducationAnim(ccm, "act_childhood_grit", "", "carry_hammer")),
        new NarrativeOption("education_artisan_guild_option", new TextObject("{=bab_education_artisan_guild_title}ran errands for the guild."), new TextObject("{=bab_education_artisan_guild_desc}You carried orders and payments between workshops, met suppliers and customers, and learned how masters defended their privileges in town."), (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Social), IsArtisan, ccm => SetEducationAnim(ccm, "act_childhood_peddlers_2", "", "_to_carry_bd_fabric_c")),
        new NarrativeOption("education_farmer_fields_option", new TextObject("{=bab_education_farmer_fields_title}worked the fields beside your family."), new TextObject("{=bab_education_farmer_fields_desc}You learned when to plough, sow, weed, and harvest, and developed the endurance to labour from first light until dusk."), (args, u) => u.UpgradeAthleticsPolearmSkills(args, DefaultCharacterAttributes.Endurance), IsFarmer, ccm => SetEducationAnim(ccm, "act_childhood_grit", "", "carry_bostaff_rogue1")),
        new NarrativeOption("education_farmer_herder_option", new TextObject("{=RKVNvimC}herded the sheep."), new TextObject("{=KfaqPpbK}You went with other fleet-footed youths to take the villages' sheep, goats or cattle to graze in pastures near the village. You were in charge of chasing down stray beasts, and always kept a big stone on hand to be hurled at lurking predators if necessary."), (args, u) => u.UpgradeAthleticsThrowingSkills(args, DefaultCharacterAttributes.Control), IsFarmer, ccm => SetEducationAnim(ccm, "act_childhood_streets", "", "carry_bostaff_rogue1")),
        new NarrativeOption("education_farmer_market_option", new TextObject("{=qAbMagWq}sold product at the market."), new TextObject("{=DIgsfYfz}You took your family's goods to the nearest town to sell your produce and buy supplies. It was hard work, but you enjoyed the hubbub of the marketplace."), (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Social), IsFarmer, ccm => SetEducationAnim(ccm, "act_childhood_peddlers_2", "", "_to_carry_bd_fabric_c")),
        new NarrativeOption("education_hunter_tracking_option", new TextObject("{=T7m7ReTq}hunted small game."), new TextObject("{=bab_education_hunter_tracking_desc}You accompanied your family into the wild, learning to read tracks, set snares, approach game quietly, and find the way home in poor weather."), (args, u) => u.UpgradeScoutingBowSkills(args, DefaultCharacterAttributes.Cunning), IsHunter, ccm => SetEducationAnim(ccm, "act_childhood_sharp", "", "composite_bow")),
        new NarrativeOption("education_hunter_traps_option", new TextObject("{=bab_education_hunter_traps_title}checked the distant traplines."), new TextObject("{=bab_education_hunter_traps_desc}You travelled alone between hidden snares and camps, learning the habits of beasts and the signs left by strangers in the wild."), (args, u) => u.UpgradeScoutingTacticsSkills(args, DefaultCharacterAttributes.Cunning), IsHunter, ccm => SetEducationAnim(ccm, "act_childhood_fox", "", "")),
        new NarrativeOption("education_hunter_herbs_option", new TextObject("{=TRwgSLD2}gathered herbs in the wild."), new TextObject("{=9ks4u5cH}You were sent by the village healer up into the hills to look for useful medicinal plants. You learned which herbs healed wounds or brought down a fever, and how to find them."), (args, u) => u.UpgradeMedicineScoutingSkills(args, DefaultCharacterAttributes.Endurance), IsHunter, ccm => SetEducationAnim(ccm, "act_childhood_peddlers", "", "_to_carry_bd_basket_a")),
    };

    public static void SetEducationAnim(CharacterCreationManager ccm, string animation, string rightHandItem, string leftHandItem)
    {
        foreach (NarrativeMenuCharacter character in ccm.CurrentMenu.Characters)
        {
            if (character.StringId == "player_education_character")
            {
                character.SetAnimationId(animation);
                character.SetLeftHandItem(leftHandItem);
                character.SetRightHandItem(rightHandItem);
                break;
            }
        }
    }
}
