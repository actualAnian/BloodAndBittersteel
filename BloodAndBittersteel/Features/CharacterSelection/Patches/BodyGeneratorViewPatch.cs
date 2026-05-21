using HarmonyLib;
using System;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
using TaleWorlds.TwoDimension;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using BloodAndBittersteel.Features.CharacterSelection.ViewModel;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
internal class BodyGeneratorViewPatch
{
    private static SpriteCategory _clanCategory;

    public static void Initialize() { }

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
            var enableCharacterSelection = true;//AoMSettings.Instance.EnableCharacterSelection;
            //_logger?.LogDebug($"BodyGeneratorViewPatch.Post_Constructor: EnableCharacterSelection={enableCharacterSelection}");

            if (!enableCharacterSelection)
                return;

            // Only show character selection during character creation, not barber screen
            if (!(GameStateManager.Current.ActiveState is CharacterCreationState))
            {
                //_logger?.LogDebug("BodyGeneratorViewPatch: Not in CharacterCreationState, skipping");
                return;
            }

            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiResourceDepot = UIResourceManager.ResourceDepot;
            _clanCategory = spriteData.SpriteCategories["ui_clan"];
            _clanCategory.Load(resourceContext, uiResourceDepot);
            CharacterSelectionViewModel charselvm = new CharacterSelectionViewModel(__instance);
            __instance.GauntletLayer.LoadMovie("PreBuildCharacterSelection", (TaleWorlds.Library.ViewModel)charselvm);
            __instance.IsDressed = true;

            //_logger?.LogDebug("BodyGeneratorViewPatch: Character selection UI loaded successfully");
        }
        catch (Exception ex)
        {
            //_logger?.LogError($"BodyGeneratorViewPatch.Post_Constructor failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BodyGeneratorView), "OnFinalize")]
    public static void Post_OnFinalize()
    {
        try
        {
            if (true)//AoMSettings.Instance.EnableCharacterSelection)
            {
                CharacterSelectionViewModel.ClearHelperStatics();

                if (_clanCategory == null)
                    return;
                _clanCategory = null;
            }
        }
        catch (Exception ex)
        {
            //_logger?.LogError($"BodyGeneratorViewPatch.Post_OnFinalize failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BodyGeneratorView), nameof(BodyGeneratorView.ResetFaceToDefault))]
    public static void Postfix()
    {
        try
        {
            if (true)//AoMSettings.Instance.EnableCharacterSelection)
            {
                CharacterSelectionViewModel.Reset();
            }
        }
        catch (Exception ex)
        {
            //_logger?.LogError($"BodyGeneratorViewPatch.Postfix (ResetFaceToDefault) failed: {ex.Message}\n{ex.StackTrace}");
        }
    }

}