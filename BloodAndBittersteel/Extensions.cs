using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
