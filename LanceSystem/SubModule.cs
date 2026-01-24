using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using LanceSystem.Deserialization;
using BloodAndBittersteel;
using LanceSystem.Models;

namespace LanceSystem
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly Harmony harmony = new("bloodandbittersteel");
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter campaignGameStarter)
            {
                campaignGameStarter.AddBehavior(new LancesCampaignBehavior());
                campaignGameStarter.AddBehavior(new AskForVolunteersCampaignBehavior());

                campaignGameStarter.AddModel(new LancePartySizeLimitModel(gameStarterObject.GetExistingModel<PartySizeLimitModel>()));
            }
        }
        protected override void OnSubModuleLoad()
        {
            harmony.PatchAll();
            LanceTemplateManager.Instance.LoadFromFile();
        }
        public override void OnGameInitializationFinished(Game game)
        {
            ManualPatchesHandler.TryRunManualPatches(harmony);
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