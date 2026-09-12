using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation;

public static class MainlandEducationOptions
{
    private static bool IsRural(CharacterCreationManager ccm) => !CharacterOccupations.IsUrbanOccupation(ccm.CharacterCreationContent.SelectedParentOccupation);
    private static bool IsUrban(CharacterCreationManager ccm) => CharacterOccupations.IsUrbanOccupation(ccm.CharacterCreationContent.SelectedParentOccupation);

    public static readonly IReadOnlyList<NarrativeOption> Shared = new NarrativeOption[]
    {
        new NarrativeOption("education_herder_option", new TextObject("{=RKVNvimC}herded the sheep."), new TextObject("{=KfaqPpbK}You went with other fleet-footed youths to take the villages' sheep, goats or cattle to graze in pastures near the village. You were in charge of chasing down stray beasts, and always kept a big stone on hand to be hurled at lurking predators if necessary."), (args, u) => u.UpgradeAthleticsThrowingSkills(args, DefaultCharacterAttributes.Control), IsRural, ccm => SetEducationAnim(ccm, "act_childhood_streets", "", "carry_bostaff_rogue1")),
        new NarrativeOption("education_smith_option", new TextObject("{=bTKiN0hr}worked in the village smithy."), new TextObject("{=y6j1bJTH}You were apprenticed to the local smith. You learned how to heat and forge metal, hammering for hours at a time until your muscles ached."), (args, u) => u.UpgradeTwoHandedCraftingSkills(args, DefaultCharacterAttributes.Vigor), IsRural, ccm => SetEducationAnim(ccm, "act_childhood_militia", "", "peasant_hammer_1_t1")),
        new NarrativeOption("education_engineer_option", new TextObject("{=tI8ZLtoA}repaired projects."), new TextObject("{=6LFj919J}You helped dig wells, rethatch houses, and fix broken plows. You learned about the basics of construction, as well as what it takes to keep a farming community prosperous."), (args, u) => u.UpgradeCraftingEngineeringSkills(args, DefaultCharacterAttributes.Intelligence), IsRural, ccm => SetEducationAnim(ccm, "act_childhood_grit", "", "carry_hammer")),
        new NarrativeOption("education_doctor_option", new TextObject("{=TRwgSLD2}gathered herbs in the wild."), new TextObject("{=9ks4u5cH}You were sent by the village healer up into the hills to look for useful medicinal plants. You learned which herbs healed wounds or brought down a fever, and how to find them."), (args, u) => u.UpgradeMedicineScoutingSkills(args, DefaultCharacterAttributes.Endurance), IsRural, ccm => SetEducationAnim(ccm, "act_childhood_peddlers", "", "_to_carry_bd_basket_a")),
        new NarrativeOption("education_hunter_option", new TextObject("{=T7m7ReTq}hunted small game."), new TextObject("{=RuvSk3QT}You accompanied a local hunter as he went into the wilderness, helping him set up traps and catch small animals."), (args, u) => u.UpgradeBowTacticsSkills(args, DefaultCharacterAttributes.Cunning), IsRural, ccm => SetEducationAnim(ccm, "act_childhood_sharp", "", "composite_bow")),
        new NarrativeOption("education_merchant_option", new TextObject("{=qAbMagWq}sold product at the market."), new TextObject("{=DIgsfYfz}You took your family's goods to the nearest town to sell your produce and buy supplies. It was hard work, but you enjoyed the hubbub of the marketplace."), (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Social), IsRural, ccm => SetEducationAnim(ccm, "act_childhood_peddlers_2", "", "_to_carry_bd_fabric_c")),
        new NarrativeOption("education_watcher_option", new TextObject("{=go7Yu7KS}watched the militia training."), new TextObject("{=qnqdEJOv}You watched the town's watch practice shooting and perfect their plans to defend the walls in case of a siege."), (args, u) => u.UpgradePolearmTacticsSkills(args, DefaultCharacterAttributes.Control), IsUrban, ccm => SetEducationAnim(ccm, "act_childhood_fox", "", "")),
        new NarrativeOption("education_ganger_option", new TextObject("{=gAjvAGTa}hung out with the gangs in the alleys."), new TextObject("{=1SUTcF0J}The gang leaders who kept watch over the slums of Calradian cities were always in need of poor youth to run messages and back them up in turf wars, while thrill-seeking merchants' sons and daughters sometimes slummed it in their company as well."), (args, u) => u.UpgradeRogueryOneHandedSkills(args, DefaultCharacterAttributes.Cunning), IsUrban, ccm => SetEducationAnim(ccm, "act_childhood_athlete", "", "")),
        new NarrativeOption("education_docker_option", new TextObject("{=QVVCgajg}helped at building sites."), new TextObject("{=bhdkegZ4}All towns had their share of projects that were constantly in need of both skilled and unskilled labor. You learned how hoists and scaffolds were constructed, how planks and stones were hewn and fitted, and other skills."), (args, u) => u.UpgradeAthleticsCraftingSkills(args, DefaultCharacterAttributes.Vigor), IsUrban, ccm => SetEducationAnim(ccm, "act_childhood_peddlers", "", "_to_carry_bd_basket_a")),
        new NarrativeOption("education_marketer_option", new TextObject("{=JTsv6PFe}worked in the markets and caravanserais."), new TextObject("{=rmMcwSn8}You helped your family handle their business affairs, going down to the marketplace to make purchases and oversee the arrival of caravans."), (args, u) => u.UpgradeTradeCharmSkills(args, DefaultCharacterAttributes.Social), IsUrban, ccm => SetEducationAnim(ccm, "act_childhood_manners", "", "")),
        new NarrativeOption("education_tutor_option", new TextObject("{=EMVojYzW}studied with your private tutor."), new TextObject("{=hXl25avg}Your family arranged for a private tutor and you took full advantage, reading voraciously on history, mathematics, and philosophy and discussing what you read with your tutor and classmates."), (args, u) => u.UpgradeEngineeringLeadershipSkills(args, DefaultCharacterAttributes.Intelligence), IsUrban, ccm => SetEducationAnim(ccm, "act_childhood_book", "character_creation_notebook", "")),
        new NarrativeOption("education_horser_option", new TextObject("{=hin3iA2D}cared for the horses."), new TextObject("{=Ghz90npw}Your family owned a few horses at the town stables and you took charge of their care. Many evenings you would take them out beyond the walls and gallup through the fields, racing other youth."), (args, u) => u.UpgradeRidingStewardSkills(args, DefaultCharacterAttributes.Endurance), IsUrban, ccm => SetEducationAnim(ccm, "act_childhood_peddlers_2", "", "_to_carry_bd_fabric_c")),
    };

    private static void SetEducationAnim(CharacterCreationManager ccm, string animation, string rightHandItem, string leftHandItem)
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
