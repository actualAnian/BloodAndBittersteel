using LanceSystem.Models;
using TaleWorlds.CampaignSystem;

namespace LanceSystem
{
    public static class GameModelExtension
    {
        private static LanceModel? _lanceModel;
        //public static LanceModel LanceModel
        //{
        //    get
        //    {
        //        return _lanceModel ??= new BaBLanceModel();
        //    }
        //}
        public static LanceModel LanceModel(this GameModels gameModel)
        {
            return _lanceModel ??= new DefaultLanceModel();
        }
    }
}
