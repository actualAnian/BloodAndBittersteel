using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace MedievalEurope15thCenturyMain.CharacterCreation
{
	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "YouthGroomOptionOnCondition")]
	public static class YouthGroomPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "YouthCavalryOptionOnCondition")]
	public static class YouthCavalryPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "YouthGuardHighRegisterOptionOnCondition")]
	public static class YouthGuardHighRegisterPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "YouthInfantryOptionOnCondition")]
	public static class YouthInfantryPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "YouthSkirmisherOptionOnCondition")]
	public static class YouthSkirmisherPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "YouthCampOptionOnCondition")]
	public static class YouthCampPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			CustomCultureOptionHelper.EnableForCustomCultures(ref __result, characterCreationManager);
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "AdulthoodManhuntOptionOnCondition")]
	public static class AdulthoodManhuntPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			// Vanilla: rural only + (vlandia/empire/aserai/battania/khuzait). Mirror vlandia for customs.
			if (__result || characterCreationManager?.CharacterCreationContent == null)
			{
				return;
			}

			string id = characterCreationManager.CharacterCreationContent.SelectedCulture?.StringId;
			if (id == null || !CustomCultureDefinitions.Ids.Contains(id))
			{
				return;
			}

			string occupation = characterCreationManager.CharacterCreationContent.SelectedParentOccupation;
			if (!IsUrbanOccupation(occupation))
			{
				__result = true;
			}
		}

		private static bool IsUrbanOccupation(string occupation)
		{
			switch (occupation)
			{
			case "retainer_urban":
			case "mercenary_urban":
			case "merchant_urban":
			case "vagabond_urban":
			case "artisan_urban":
			case "physician_urban":
			case "healer_urban":
			case "bard_urban":
				return true;
			default:
				return false;
			}
		}
	}

	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "AdulthoodCaravanLeaderOptionOnCondition")]
	public static class AdulthoodCaravanLeaderPatch
	{
		[HarmonyPostfix]
		private static void Postfix(ref bool __result, CharacterCreationManager characterCreationManager)
		{
			// Vanilla: urban only + (vlandia/sturgia/empire/aserai/khuzait/nord). Mirror for customs.
			if (__result || characterCreationManager?.CharacterCreationContent == null)
			{
				return;
			}

			string id = characterCreationManager.CharacterCreationContent.SelectedCulture?.StringId;
			if (id == null || !CustomCultureDefinitions.Ids.Contains(id))
			{
				return;
			}

			string occupation = characterCreationManager.CharacterCreationContent.SelectedParentOccupation;
			if (IsUrbanOccupation(occupation))
			{
				__result = true;
			}
		}

		private static bool IsUrbanOccupation(string occupation)
		{
			switch (occupation)
			{
			case "retainer_urban":
			case "mercenary_urban":
			case "merchant_urban":
			case "vagabond_urban":
			case "artisan_urban":
			case "physician_urban":
			case "healer_urban":
			case "bard_urban":
				return true;
			default:
				return false;
			}
		}
	}
}
