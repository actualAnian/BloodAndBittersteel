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
        if (CharacterSelectionViewModel.Instance.IsPreBuiltHero)
            CharacterCreationStagesHelper.GotoStage(typeof(CharacterCreationOptionsStage));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterCreationClanNamingStageVM), nameof(CharacterCreationClanNamingStageVM.OnPreviousStage))]
    public static void Post_OnPreviousStagee()
    {
        if (CharacterSelectionViewModel.Instance.IsPreBuiltHero)
        {
            CharacterSelectionViewModel.Instance.Reset();
            CharacterCreationStagesHelper.GotoStage(typeof(CharacterCreationFaceGeneratorStage));
        }
    }
}