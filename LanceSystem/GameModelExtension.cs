using LanceSystem.Models;
using TaleWorlds.CampaignSystem;

namespace LanceSystem
{
    public static class GameModelExtension
    {
        private static LanceModel? _lanceModel;
#pragma warning disable IDE0060 // Remove unused parameter
        public static LanceModel LanceModel(this GameModels gameModel)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return _lanceModel ??= new DefaultLanceModel();
        }
    }
}
