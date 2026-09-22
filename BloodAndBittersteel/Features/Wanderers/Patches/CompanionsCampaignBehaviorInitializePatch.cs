using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.Wanderers.Patches;

[HarmonyPatch(typeof(CompanionsCampaignBehavior), "InitializeCompanionTemplateList")]
public static class CompanionsCampaignBehaviorInitializePatch
{
    private static readonly HashSet<string> VanillaCultureIds = new()
    {
        "empire",
        "sturgia",
        "vlandia",
        "aserai",
        "battania",
        "khuzait"
    };

    public static bool Prefix(CompanionsCampaignBehavior __instance)
    {
        var companionsOfTemplates = (IDictionary)AccessTools
            .Field(typeof(CompanionsCampaignBehavior), "_companionsOfTemplates")
            .GetValue(__instance);

        var getTemplateType = AccessTools.Method(typeof(CompanionsCampaignBehavior), "GetTemplateTypeOfCompanion");

        foreach (CharacterObject objectType in MBObjectManager.Instance.GetObjectTypeList<CharacterObject>())
        {
            if (objectType.IsTemplate && objectType.Occupation == Occupation.Wanderer && !VanillaCultureIds.Contains(objectType.Culture.StringId))
            {
                var templateType = getTemplateType.Invoke(__instance, new object[] { objectType });
                var list = (List<CharacterObject>)companionsOfTemplates[templateType];
                list.Add(objectType);
            }
        }
        return false;
    }
}
