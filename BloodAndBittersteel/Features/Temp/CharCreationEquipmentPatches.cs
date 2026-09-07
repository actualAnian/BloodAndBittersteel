using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace MedievalEurope15thCenturyMain.CharacterCreation
{
	internal static class CharCreationEquipmentHelper
	{
		public const string FallbackCultureSuffix = "_vlandia";

		public static void RemapCustomCultureSuffix(ref string result, string cultureId)
		{
			if (string.IsNullOrEmpty(result) || string.IsNullOrEmpty(cultureId))
			{
				return;
			}

			if (!CustomCultureDefinitions.Ids.Contains(cultureId))
			{
				return;
			}

			foreach (string id in CustomCultureDefinitions.Ids)
			{
				result = result.Replace("_" + id, FallbackCultureSuffix);
			}
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "GetMotherEquipmentId")]
	public static class MotherEquipmentPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref string __result, string cultureId)
		{
			CharCreationEquipmentHelper.RemapCustomCultureSuffix(ref __result, cultureId);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "GetFatherEquipmentId")]
	public static class FatherEquipmentPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref string __result, string cultureId)
		{
			CharCreationEquipmentHelper.RemapCustomCultureSuffix(ref __result, cultureId);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "GetPlayerChildhoodAgeEquipmentId")]
	public static class PlayerChildhoodEquipmentPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref string __result, string cultureId)
		{
			CharCreationEquipmentHelper.RemapCustomCultureSuffix(ref __result, cultureId);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "GetPlayerEducationAgeEquipmentId")]
	public static class PlayerEducationEquipmentPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref string __result, string cultureId)
		{
			CharCreationEquipmentHelper.RemapCustomCultureSuffix(ref __result, cultureId);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "GetPlayerEquipmentId")]
	public static class PlayerEquipmentPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref string __result, string cultureId)
		{
			CharCreationEquipmentHelper.RemapCustomCultureSuffix(ref __result, cultureId);
		}
	}
}
