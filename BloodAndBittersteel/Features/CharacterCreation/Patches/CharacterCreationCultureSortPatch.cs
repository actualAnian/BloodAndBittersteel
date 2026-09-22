using HarmonyLib;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.CharacterCreation.Patches
{
    [HarmonyPatch(typeof(CharacterCreationCultureStageVM), "SortCultureList")]
    public static class CharacterCreationCultureSortPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(MBBindingList<CharacterCreationCultureVM> listToWorkOn)
        {
            // Custom safe sort: only move cultures that exist
            var desiredOrder = new[] { "crown", "vale", "storm", "dorn", "reach", "wester", "river", "iron", "north" };
            int targetIndex = 0;
            foreach (var partialId in desiredOrder)
            {
                var match = listToWorkOn.FirstOrDefault(i => i.CultureID.Contains(partialId));
                if (match != null)
                {
                    int currentIndex = listToWorkOn.IndexOf(match);
                    if (currentIndex != targetIndex)
                    {
                        var item = listToWorkOn[targetIndex];
                        listToWorkOn[targetIndex] = listToWorkOn[currentIndex];
                        listToWorkOn[currentIndex] = item;
                    }
                    targetIndex++;
                }
            }
            return false;
        }
    }
}
