using BloodAndBittersteel.Features.LanceSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BloodAndBittersteel
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
            return _lanceModel ??= new BaBLanceModel();
        }
    }
}
