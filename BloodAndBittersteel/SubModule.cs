using BloodAndBittersteel.Features.BaBIncidents;
using BloodAndBittersteel.Features.BlackfyreRebellion;
using BloodAndBittersteel.Models;
using HarmonyLib;
using SandBox.ViewModelCollection.Map.Incidents;
using System.Linq;
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
                
                // models
                campaignGameStarter.AddModel(new BnBCampaignTimeModel());
                campaignGameStarter.AddModel(new BnBMapWeatherModel(gameStarterObject.GetExistingModel<MapWeatherModel>()));
            }
        }
        protected override void OnSubModuleLoad()
        {
            var ctor = AccessTools.GetDeclaredConstructors(typeof(MapIncidentVM));
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