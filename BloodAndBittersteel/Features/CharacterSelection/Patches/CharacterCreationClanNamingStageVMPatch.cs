using BloodAndBittersteel.Features.CharacterSelection.Helper;
using BloodAndBittersteel.Features.CharacterSelection.ViewModel;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
public static class CharacterCreationClanNamingStageVMPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterCreationClanNamingStageVM), nameof(CharacterCreationClanNamingStageVM.OnNextStage))]
    public static void Post_OnNextStage()
    {
        if (CharacterSelectionViewModel.IsPreBuildHero)
        {
            CharacterCreationStagesHelper.GotoStage(typeof(CharacterCreationOptionsStage));
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterCreationClanNamingStageVM), nameof(CharacterCreationClanNamingStageVM.OnPreviousStage))]
    public static void Post_OnPreviousStagee()
    {
        if (CharacterSelectionViewModel.IsPreBuildHero)
        {
            CharacterSelectionViewModel.Reset();
            CharacterCreationStagesHelper.GotoStage(typeof(CharacterCreationFaceGeneratorStage));
        }
    }

}