using BloodAndBittersteel.Features;
using BloodAndBittersteel.Features.BaBIncidents;
using BloodAndBittersteel.Features.BlackfyreRebellion;
using BloodAndBittersteel.Features.Tribute;
using BloodAndBittersteel.Models;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BloodAndBittersteel
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly Harmony harmony = new("bloodandbittersteel");
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter campaignGameStarter)
            {
                // behaviors
                campaignGameStarter.AddBehavior(new BaBIncidentsCampaignBehavior());
                campaignGameStarter.AddBehavior(new RebellionCampaignBehavior());
                campaignGameStarter.AddBehavior(new BaBDailyTribute());
                
                // models
                campaignGameStarter.AddModel(new BaBCampaignTimeModel());
                campaignGameStarter.AddModel(new BaBMapWeatherModel(gameStarterObject.GetExistingModel<MapWeatherModel>()));
                campaignGameStarter.AddModel(new BaBKingdomDecisionPermissionModel(gameStarterObject.GetExistingModel<KingdomDecisionPermissionModel>()));
            }
        }
        //public override void OnCampaignStart(Game game, object starterObject)
        //{
        //    var a = Kingdom.All;
        //    foreach (var k1 in Kingdom.All)
        //    {
        //        foreach (var k2 in Kingdom.All)
        //        {
        //            var link = k1.GetStanceWith(k2);
        //        }
        //    }
        //}
        protected override void OnSubModuleLoad()
        {
            harmony.PatchAll();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
        }
    }
}