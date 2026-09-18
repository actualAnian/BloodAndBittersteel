using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.CharacterCreation;

public class NarrativeSkillUpgrades
{
    public int FocusToAdd { get; }
    public int SkillLevelToAdd { get; }
    public int AttributeLevelToAdd { get; }

    public NarrativeSkillUpgrades(CharacterCreationManager ccm)
    {
        FocusToAdd = ccm.CharacterCreationContent.FocusToAdd;
        SkillLevelToAdd = ccm.CharacterCreationContent.SkillLevelToAdd;
        AttributeLevelToAdd = ccm.CharacterCreationContent.AttributeLevelToAdd;
    }

    public void UpgradeRidingThrowingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Riding, DefaultSkills.Throwing }, attr);

    public void UpgradeRidingPolearmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Riding, DefaultSkills.Polearm }, attr);

    public void UpgradeRidingTwoHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Riding, DefaultSkills.TwoHanded }, attr);

    public void UpgradeRidingBowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Riding, DefaultSkills.Bow }, attr);

    public void UpgradeTradeCharmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Trade, DefaultSkills.Charm }, attr);

    public void UpgradeTradeTacticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Trade, DefaultSkills.Tactics }, attr);

    public void UpgradeTradeCraftingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Trade, DefaultSkills.Crafting }, attr);

    public void UpgradeAthleticsPolearmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Athletics, DefaultSkills.Polearm }, attr);

    public void UpgradeAthleticsOneHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Athletics, DefaultSkills.OneHanded }, attr);

    public void UpgradeAthleticsThrowingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Athletics, DefaultSkills.Throwing }, attr);

    public void UpgradeAthleticsBowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Athletics, DefaultSkills.Bow }, attr);

    public void UpgradeAthleticsCraftingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Athletics, DefaultSkills.Crafting }, attr);

    public void UpgradeAthleticsRoguerySkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Athletics, DefaultSkills.Roguery }, attr);

    public void UpgradeCraftingCrossbowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Crafting, DefaultSkills.Crossbow }, attr);

    public void UpgradeCraftingTwoHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Crafting, DefaultSkills.TwoHanded }, attr);

    public void UpgradeCraftingOneHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Crafting, DefaultSkills.OneHanded }, attr);

    public void UpgradeCraftingEngineeringSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Crafting, DefaultSkills.Engineering }, attr);

    public void UpgradeScoutingBowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Scouting, DefaultSkills.Bow }, attr);

    public void UpgradeScoutingCrossbowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Scouting, DefaultSkills.Crossbow }, attr);

    public void UpgradeScoutingTacticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Scouting, DefaultSkills.Tactics }, attr);

    public void UpgradeScoutingRidingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Scouting, DefaultSkills.Riding }, attr);

    public void UpgradeRogueryThrowingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Roguery, DefaultSkills.Throwing }, attr);

    public void UpgradeRogueryCrossbowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Roguery, DefaultSkills.Crossbow }, attr);

    public void UpgradeRogueryPolearmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Roguery, DefaultSkills.Polearm }, attr);

    public void UpgradeRogueryOneHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Roguery, DefaultSkills.OneHanded }, attr);

    public void UpgradeRogueryCharmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Roguery, DefaultSkills.Charm }, attr);

    public void UpgradeMedicineCharmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Medicine, DefaultSkills.Charm }, attr);

    public void UpgradeMedicineScoutingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Medicine, DefaultSkills.Scouting }, attr);

    public void UpgradeTwoHandedBowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.TwoHanded, DefaultSkills.Bow }, attr);

    public void UpgradePolearmCrossbowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Polearm, DefaultSkills.Crossbow }, attr);

    public void UpgradeBowRidingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Bow, DefaultSkills.Riding }, attr);

    public void UpgradePolearmThrowingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Polearm, DefaultSkills.Throwing }, attr);

    public void UpgradePolearmOneHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Polearm, DefaultSkills.OneHanded }, attr);

    public void UpgradeThrowingOneHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Throwing, DefaultSkills.OneHanded }, attr);

    public void UpgradeOneHandedTwoHandedSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.OneHanded, DefaultSkills.TwoHanded }, attr);

    public void UpgradeBowCrossbowSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Bow, DefaultSkills.Crossbow }, attr);

    public void UpgradeStewardTacticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Steward, DefaultSkills.Tactics }, attr);

    public void UpgradeStewardCharmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Steward, DefaultSkills.Charm }, attr);

    public void UpgradeCharmTacticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Charm, DefaultSkills.Tactics }, attr);

    public void UpgradeCharmScoutingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Charm, DefaultSkills.Scouting }, attr);

    public void UpgradeCrossbowEngineeringSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Crossbow, DefaultSkills.Engineering }, attr);

    public void UpgradeBowEngineeringSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Bow, DefaultSkills.Engineering }, attr);

    public void UpgradeLeadershipTacticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Leadership, DefaultSkills.Tactics }, attr);

    public void UpgradeLeadershipCharmSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Leadership, DefaultSkills.Charm }, attr);

    public void UpgradeLeadershipStewardSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Leadership, DefaultSkills.Steward }, attr);

    public void UpgradeEngineeringLeadershipSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Engineering, DefaultSkills.Leadership }, attr);

    public void UpgradeRidingMedicineSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Riding, DefaultSkills.Medicine }, attr);

    public void UpgradeRidingStewardSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Riding, DefaultSkills.Steward }, attr);

    public void UpgradeTwoHandedCraftingSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.TwoHanded, DefaultSkills.Crafting }, attr);

    public void UpgradeEngineeringTradeSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Engineering, DefaultSkills.Trade }, attr);

    public void UpgradeBowTacticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Bow, DefaultSkills.Tactics }, attr);

    public void UpgradePolearmTacticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Polearm, DefaultSkills.Tactics }, attr);

    public void UpgradeTradeLeadershipSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Trade, DefaultSkills.Leadership }, attr);

    public void UpgradePolearmAthleticsSkills(NarrativeMenuOptionArgs args, CharacterAttribute attr)
        => Apply(args, new[] { DefaultSkills.Polearm, DefaultSkills.Athletics }, attr);

    private void Apply(NarrativeMenuOptionArgs args, SkillObject[] skills, CharacterAttribute attr)
    {
        args.SetAffectedSkills(skills);
        args.SetFocusToSkills(FocusToAdd);
        args.SetLevelToSkills(SkillLevelToAdd);
        args.SetLevelToAttribute(attr, AttributeLevelToAdd);
    }
}
