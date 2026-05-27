using HarmonyLib;
using System;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using BloodAndBittersteel.Features.CharacterSelection.ViewModel;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
internal class BodyGeneratorViewPatch
{

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BodyGeneratorView), MethodType.Constructor, new Type[]
    {
        typeof(ControlCharacterCreationStage),
        typeof(TextObject),
        typeof(ControlCharacterCreationStage),
        typeof(TextObject),
        typeof(BasicCharacterObject),
        typeof(bool),
        typeof(IFaceGeneratorCustomFilter),
        typeof(Equipment),
        typeof(ControlCharacterCreationStageReturnInt),
        typeof(ControlCharacterCreationStageReturnInt),
        typeof(ControlCharacterCreationStageReturnInt),
        typeof(ControlCharacterCreationStageWithInt),
        typeof(FaceGenHistory)
    })]
    public static void Post_Constructor(ref BodyGeneratorView __instance)
    {
        try
        {
            // Only show character selection during character creation, not barber screen
            if (GameStateManager.Current.ActiveState is not CharacterCreationState) return;

            var spriteData = UIResourceManager.SpriteData;
            var resourceContext = UIResourceManager.ResourceContext;
            var uiResourceDepot = UIResourceManager.ResourceDepot;
            var clanCategory = spriteData.SpriteCategories["ui_clan"];
            clanCategory.Load(resourceContext, uiResourceDepot);
            var charselvm = new CharacterSelectionViewModel(__instance);
            __instance.GauntletLayer.LoadMovie("PreBuildCharacterSelection", charselvm);
            __instance.IsDressed = true;
        }
        catch (Exception)
        {
            //_logger?.LogError($"BodyGeneratorViewPatch.Post_Constructor failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BodyGeneratorView), nameof(BodyGeneratorView.ResetFaceToDefault))]
    public static void Postfix()
    {
        CharacterSelectionViewModel.Instance.Reset();
    }
}