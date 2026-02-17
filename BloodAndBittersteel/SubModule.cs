using BloodAndBittersteel.Features.BaBIncidents;
using BloodAndBittersteel.Features.BlackfyreRebellion;
using BloodAndBittersteel.Features.CampaignStart;
using BloodAndBittersteel.Features.Tribute;
using BloodAndBittersteel.Models;
using HarmonyLib;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
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
                campaignGameStarter.AddModel(new BaBSettlementAccessModel(gameStarterObject.GetExistingModel<SettlementAccessModel>()));
                campaignGameStarter.AddModel(new BaBSettlementLoyaltyModel(gameStarterObject.GetExistingModel<SettlementLoyaltyModel>()));
                // lance system defines PartySizeLimitModel

                RemoveChildGenerationAtGameStart(campaignGameStarter);
                ToggleInformationRestrictionModel(campaignGameStarter);
                // temporary, before it implements proper characterCreation interface
                campaignGameStarter.AddBehavior(new BaBCampaignStartBehavior());
            }
        }
        private void RemoveChildGenerationAtGameStart(CampaignGameStarter campaignGameStarter)
        {
            var behavior = campaignGameStarter.CampaignBehaviors.First(b => b is InitialChildGenerationCampaignBehavior);
            campaignGameStarter.RemoveBehavior(behavior);
        }
        private void ToggleInformationRestrictionModel(CampaignGameStarter campaignGameStarter)
        {
            DefaultInformationRestrictionModel defaultInformationRestrictionModel = (DefaultInformationRestrictionModel)campaignGameStarter.Models.First(m => m is DefaultInformationRestrictionModel);

            defaultInformationRestrictionModel.IsDisabledByCheat = true;
        }
        protected override void OnSubModuleLoad()
        {
            harmony.PatchAll();
        }
        public override void OnGameInitializationFinished(Game game)
        {
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