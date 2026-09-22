namespace BloodAndBittersteel.Features.CharacterCreation;

public enum NarrativeChoiceStage
{
    Parent,
    Childhood,
    Education,
    Youth,
    Adulthood
}

/// <summary>
/// Records the current character-creation route so later narrative stages can
/// condition options on the exact choices made in earlier stages. Selecting a
/// different option after navigating back simply replaces that stage's value.
/// </summary>
public static class NarrativeChoiceState
{
    public static string ParentOptionId { get; private set; } = string.Empty;
    public static string ChildhoodOptionId { get; private set; } = string.Empty;
    public static string EducationOptionId { get; private set; } = string.Empty;
    public static string YouthOptionId { get; private set; } = string.Empty;
    public static string AdulthoodOptionId { get; private set; } = string.Empty;

    public static void Reset()
    {
        ParentOptionId = string.Empty;
        ChildhoodOptionId = string.Empty;
        EducationOptionId = string.Empty;
        YouthOptionId = string.Empty;
        AdulthoodOptionId = string.Empty;
    }

    public static void Record(NarrativeChoiceStage stage, string optionId)
    {
        switch (stage)
        {
            case NarrativeChoiceStage.Parent:
                ParentOptionId = optionId;
                ChildhoodOptionId = string.Empty;
                EducationOptionId = string.Empty;
                YouthOptionId = string.Empty;
                AdulthoodOptionId = string.Empty;
                break;
            case NarrativeChoiceStage.Childhood:
                ChildhoodOptionId = optionId;
                EducationOptionId = string.Empty;
                YouthOptionId = string.Empty;
                AdulthoodOptionId = string.Empty;
                break;
            case NarrativeChoiceStage.Education:
                EducationOptionId = optionId;
                YouthOptionId = string.Empty;
                AdulthoodOptionId = string.Empty;
                break;
            case NarrativeChoiceStage.Youth:
                YouthOptionId = optionId;
                AdulthoodOptionId = string.Empty;
                break;
            case NarrativeChoiceStage.Adulthood:
                AdulthoodOptionId = optionId;
                break;
        }
    }

    public static bool ChildhoodWas(params string[] optionIds) => Matches(ChildhoodOptionId, optionIds);
    public static bool EducationWas(params string[] optionIds) => Matches(EducationOptionId, optionIds);
    public static bool YouthWas(params string[] optionIds) => Matches(YouthOptionId, optionIds);

    private static bool Matches(string selected, string[] optionIds)
    {
        foreach (string optionId in optionIds)
        {
            if (selected == optionId)
                return true;
        }
        return false;
    }
}
