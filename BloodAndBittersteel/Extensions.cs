using System.Linq;
using TaleWorlds.Core;

namespace BloodAndBittersteel
{
    public static class Extensions
    {
        public static TBaseModel GetExistingModel<TBaseModel>(this IGameStarter campaignGameStarter) where TBaseModel : GameModel
        {
            return (TBaseModel)campaignGameStarter.Models.First(model => model.GetType().IsSubclassOf(typeof(TBaseModel)));
        }

    }
}
