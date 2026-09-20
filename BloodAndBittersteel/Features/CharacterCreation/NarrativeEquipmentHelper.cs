using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.CharacterCreation;

public static class NarrativeEquipmentHelper
{
    private const string FallbackEquipmentId = "player_char_creation_empire_retainer_m";

    public static MBEquipmentRoster LoadWithFallback(string equipmentId)
    {
        var roster = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(equipmentId);
        if (roster == null)
        {
            InformationManager.DisplayMessage(new($"ERROR, no equipment with id {equipmentId}", new Color(1, 0, 0)));
            return LoadFallback();
        }
        return roster;
    }

    private static MBEquipmentRoster LoadFallback()
    {
        return Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(FallbackEquipmentId);
    }

    public static string GetMotherEquipmentId(CharacterCreationManager ccm, string occupationType, string cultureId)
    {
        ccm.CharacterCreationContent.TryGetEquipmentToUse(occupationType, out var equipmentId);
        return "mother_char_creation_" + equipmentId + "_" + cultureId;
    }

    public static string GetFatherEquipmentId(CharacterCreationManager ccm, string occupationType, string cultureId)
    {
        ccm.CharacterCreationContent.TryGetEquipmentToUse(occupationType, out var equipmentId);
        return "father_char_creation_" + equipmentId + "_" + cultureId;
    }

    public static string GetPlayerChildhoodAgeEquipmentId(CharacterCreationManager ccm, string parentOccupationType, string cultureId, bool isFemale)
    {
        ccm.CharacterCreationContent.TryGetEquipmentToUse(parentOccupationType, out var equipmentId);
        return "player_char_creation_childhood_age_" + cultureId + "_" + equipmentId + "_" + (isFemale ? "f" : "m");
    }

    public static string GetPlayerEducationAgeEquipmentId(CharacterCreationManager ccm, string parentOccupationType, string cultureId, bool isFemale)
    {
        ccm.CharacterCreationContent.TryGetEquipmentToUse(parentOccupationType, out var equipmentId);
        return "player_char_creation_education_age_" + cultureId + "_" + equipmentId + "_" + (isFemale ? "f" : "m");
    }

    public static string GetPlayerEquipmentId(CharacterCreationManager ccm, string occupationType, string cultureId, bool isFemale)
    {
        ccm.CharacterCreationContent.TryGetEquipmentToUse(occupationType, out var equipmentId);
        return "player_char_creation_" + cultureId + "_" + equipmentId + "_" + (isFemale ? "f" : "m");
    }

    public static void SetParentEquipment(CharacterCreationManager ccm, string motherEquipmentId, string fatherEquipmentId, string motherAnimation, string fatherAnimation)
    {
        MBEquipmentRoster motherEquipment = LoadWithFallback(motherEquipmentId);
        MBEquipmentRoster fatherEquipment = LoadWithFallback(fatherEquipmentId);
        foreach (NarrativeMenuCharacter character in ccm.CurrentMenu.Characters)
        {
            if (character.StringId.Equals("mother_character"))
            {
                character.SetEquipment(motherEquipment);
                character.SetAnimationId(motherAnimation);
            }
            if (character.StringId.Equals("father_character"))
            {
                character.SetEquipment(fatherEquipment);
                character.SetAnimationId(fatherAnimation);
            }
        }
    }

    public static void SetPlayerEquipment(CharacterCreationManager ccm, string equipmentId, string animation)
    {
        MBEquipmentRoster equipment = LoadWithFallback(equipmentId);
        foreach (NarrativeMenuCharacter character in ccm.CurrentMenu.Characters)
        {
            if (character.StringId == "player_youth_character" || character.StringId == "player_adulthood_character" || character.StringId == "player_age_selection_character")
            {
                character.SetAnimationId(animation);
                character.SetEquipment(equipment);
            }
        }
    }

    public static void SetPlayerCharacterAnimation(CharacterCreationManager ccm, string characterStringId, string animation)
    {
        foreach (NarrativeMenuCharacter character in ccm.CurrentMenu.Characters)
        {
            if (character.StringId == characterStringId)
                character.SetAnimationId(animation);
        }
    }
}
