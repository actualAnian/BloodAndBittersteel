using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace MedievalEurope15thCenturyMain.CharacterCreation
{
	internal static class CustomCultureOptionHelper
	{
		public static void EnableForCustomCultures(ref bool result, CharacterCreationManager characterCreationManager)
		{
			string id = characterCreationManager?.CharacterCreationContent?.SelectedCulture?.StringId;
			if (id != null && CustomCultureDefinitions.Ids.Contains(id))
			{
				result = true;
			}
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "VlandiaRetainerNarrativeOptionOnCondition")]
	public static class VlandiaRetainerPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "VlandiaMerchantNarrativeOptionOnCondition")]
	public static class VlandiaMerchantPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "VlandiaFarmerNarrativeOptionOnCondition")]
	public static class VlandiaFarmerPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "VlandiaBlacksmithNarrativeOptionOnCondition")]
	public static class VlandiaBlacksmithPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "VlandiaHunterNarrativeOptionOnCondition")]
	public static class VlandiaHunterPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "VlandiaMercenaryNarrativeOptionOnCondition")]
	public static class VlandiaMercenaryPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}
}
