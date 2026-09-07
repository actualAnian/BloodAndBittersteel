using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;

namespace MedievalEurope15thCenturyMain.CharacterCreation
{
	/// <summary>
	/// Vanilla only adds the six main cultures; append our custom ones (LT / ME15 pattern).
	/// </summary>
	[HarmonyPatch(typeof(CharacterCreationCampaignBehavior), nameof(CharacterCreationCampaignBehavior.InitializeCharacterCreationCultures))]
	public static class AddNewCulturePatch
	{
		[HarmonyPostfix]
		public static void Postfix(CharacterCreationManager characterCreationManager)
		{
			if (characterCreationManager?.CharacterCreationContent == null || Game.Current?.ObjectManager == null)
			{
				return;
			}

			foreach (CultureObject culture in Game.Current.ObjectManager.GetObjectTypeList<CultureObject>())
			{
				if (CustomCultureDefinitions.Ids.Contains(culture.StringId))
				{
					characterCreationManager.CharacterCreationContent.AddCharacterCreationCulture(culture, 1, 10);
				}
			}
		}
	}
}
