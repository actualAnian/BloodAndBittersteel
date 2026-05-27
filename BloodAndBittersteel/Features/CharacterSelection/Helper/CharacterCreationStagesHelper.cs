using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.CharacterSelection.Helper;

internal static class CharacterCreationStagesHelper
{
    public static CharacterCreationState CurrentState => (CharacterCreationState)GameStateManager.Current.ActiveState;

    public static MBList<CharacterCreationStageBase> GetStages()
    {
        CharacterCreationManager manager = (GameStateManager.Current.ActiveState as CharacterCreationState)!.CharacterCreationManager;
        return ReflectionHelper.GetFieldValue<CharacterCreationManager, MBList<CharacterCreationStageBase>>(manager, "_stages");
    }

    public static void GotoStage(Type stagetype)
    {
        var stages = GetStages();
        int nextIndex = stages.FindIndex(a => a.GetType() == stagetype);
        //ReflectionHelper.SetFieldValue(CurrentState, "_furthestStageIndex", nextIndex);
        CurrentState.CharacterCreationManager.GoToStage(nextIndex);
    }
}