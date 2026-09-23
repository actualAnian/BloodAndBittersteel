using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.CharacterCreation;

public record NarrativeOption
{
    public string OptionId { get; }
    public TextObject Title { get; }
    public TextObject Description { get; }
    public Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> SetArgs { get; }
    public Func<CharacterCreationManager, bool> Condition { get; }
    public Action<CharacterCreationManager> OnSelect { get; }

    public NarrativeOption(string optionId, TextObject title, TextObject description, Action<NarrativeMenuOptionArgs, NarrativeSkillUpgrades> setArgs, Func<CharacterCreationManager, bool> condition, Action<CharacterCreationManager> onSelect)
    {
        OptionId = optionId;
        Title = title;
        Description = description;
        SetArgs = setArgs;
        Condition = condition;
        OnSelect = onSelect;
    }
}
