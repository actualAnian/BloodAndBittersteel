using HarmonyLib;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator;

namespace BloodAndBittersteel.Features.CharacterSelection.Patches;

[HarmonyPatch]
internal class FaceGenPropertyVMPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(FaceGenPropertyVM), "Name", MethodType.Getter)]
    public static bool Prefix(ref string __result, ref FaceGenPropertyVM __instance)
    {
        TextObject textObject = ReflectionHelper.GetFieldValue<FaceGenPropertyVM, TextObject>(__instance, "_nameObj");
        float num = ReflectionHelper.GetFieldValue<FaceGenPropertyVM, float>(__instance, "_value");
        if (textObject != null)
            __result = $"{textObject.ToString()}({num.ToString("F2")})";
        return false;
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(FaceGenPropertyVM), "Value", MethodType.Setter)]
    public static void Postfix(ref FaceGenPropertyVM __instance)
    {
        ((TaleWorlds.Library.ViewModel)__instance).OnPropertyChanged("Name");
    }
}