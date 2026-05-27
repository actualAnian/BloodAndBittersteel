using HarmonyLib;
using TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator;
using BloodAndBittersteel.Features.CharacterSelection.ViewModel;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
public static class FaceGeneratorVMPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FaceGenVM), nameof(FaceGenVM.ExecuteDone))]
    public static void Post_ExecuteDone()
    {
        if (CharacterSelectionViewModel.Instance.IsPreBuiltHero)
            CharacterSelectionViewModel.Instance.ExcuteActionForPrebuildHero();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FaceGenVM), nameof(FaceGenVM.ExecuteCancel))]
    public static void Post_ExecuteCancel()
    {
        if (CharacterSelectionViewModel.Instance.IsPreBuiltHero)
            CharacterSelectionViewModel.Instance.Reset();
    }
}