using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator;
using BloodAndBittersteel.Features.CharacterSelection.ViewModel;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
public static class FaceGeneratorVMPatch
{
    private static SelectorVM<SelectorItemVM> _allracesselector;

    static FaceGeneratorVMPatch()
    {
        _allracesselector = null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FaceGenVM), nameof(FaceGenVM.ExecuteDone))]
    public static void Post_ExecuteDone()
    {
        if (CharacterSelectionViewModel.IsPreBuildHero)
        {
            CharacterSelectionViewModel.ExcuteActionForPrebuildHero();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FaceGenVM), nameof(FaceGenVM.ExecuteCancel))]
    public static void Post_ExecuteCancel()
    {
        if (CharacterSelectionViewModel.IsPreBuildHero)
        {
            CharacterSelectionViewModel.Reset();
        }
    }

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(FaceGenVM), nameof(FaceGenVM.RaceSelector), MethodType.Getter)]
    //public static void Post_RaceSelector(FaceGenVM __instance, ref SelectorVM<SelectorItemVM> __result)
    //{
    //    if (CharacterSelectionViewModel.IsPreBuildHero)
    //    {
    //        CheckAllRacesSelector(__result);
    //        ResetVoices(__instance);

    //        __result = _allracesselector;
    //    }
    //}

    //private static void CheckAllRacesSelector(SelectorVM<SelectorItemVM> __result)
    //{
    //    if (_allracesselector == null)
    //    {
    //        var allraces = IoC.Resolve<IRaceManager>().GetAllRaceNames();
    //        var onchangevent = ReflectionHelper.GetFieldValue<SelectorVM<SelectorItemVM>, Action<SelectorVM<SelectorItemVM>>>(__result, "_onChange");

    //        _allracesselector = new SelectorVM<SelectorItemVM>(allraces, CharacterSelectionViewModel.PreBuildHero.CharacterObject.Race, onchangevent);
    //    }
    //}

    //private static void ResetVoices(FaceGenVM __instance)
    //{
    //    var voices = MBBodyProperties.GetVoiceTypeUsableForPlayerData(CharacterSelectionViewModel.PreBuildHero.CharacterObject.Race, CharacterSelectionViewModel.PreBuildHero.CharacterObject.IsFemale ? 1 : 0, (int)CharacterSelectionViewModel.PreBuildHero.Age, 10);
    //    for (int i = 0; i < voices.Count; i++)
    //    {
    //        voices[i] = false;
    //    }

    //    ReflectionHelper.SetFieldValue(__instance, "_isVoiceTypeUsableForOnlyNpc", voices);
    //}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FaceGenVM), nameof(FaceGenVM.SetBodyProperties))]
    public static void Pre_SetBodyProperties(FaceGenVM __instance, BodyProperties bodyProperties, bool ignoreDebugValues, int race = 0, int gender = -1, bool recordChange = false)
    {
        if (CharacterSelectionViewModel.IsPreBuildHero)
        {
            //if (__instance.RaceSelector.ItemList.Count != _allracesselector.ItemList.Count)
            //{
            //    ReflectionHelper.SetFieldValue(__instance, "RaceSelector", _allracesselector);
            //}
            __instance.RaceSelector.SelectedIndex = race;
            //ResetVoices(__instance);
        }
    }

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(FaceGenVM), nameof(FaceGenVM.ExecuteChangeClothing))]
    //public static void Getter_ExecuteChangeClothing(ref FaceGenVM __instance)
    //{
    //    if (CharacterSelectionViewModel.IsPreBuildHero)
    //    {
    //        if (RaceCheckHelper.CheckRaceName("nazghul", CharacterSelectionViewModel.PreBuildHero.CharacterObject))
    //        {
    //            var handler = ReflectionHelper.GetFieldValue<FaceGenVM, IFaceGeneratorHandler>(__instance, "_faceGeneratorScreen");
    //            handler.DressCharacterEntity();
    //            __instance.IsDressed = true;
    //        }
    //    }
    //}
}