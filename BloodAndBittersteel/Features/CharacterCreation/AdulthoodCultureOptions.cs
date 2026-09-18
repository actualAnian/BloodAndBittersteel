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

    private static readonly string[] ManhuntCultures = { "vlandia", "empire", "aserai", "battania", "khuzait" };
    private static readonly string[] CaravanLeaderCultures = { "vlandia", "sturgia", "empire", "aserai", "khuzait" };
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
}
