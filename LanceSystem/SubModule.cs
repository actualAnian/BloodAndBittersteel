using BloodAndBittersteel;
using HarmonyLib;
using LanceSystem.CampaignBehaviors;
using LanceSystem.Deserialization;
using LanceSystem.DynamicTroops;
using LanceSystem.MCM;
using LanceSystem.Models;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace LanceSystem
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly Harmony harmony = new("bloodandbittersteel");
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            LanceEvents.RemoveAllListeners();
            if (gameStarterObject is CampaignGameStarter campaignGameStarter)
            {
                campaignGameStarter.AddBehavior(new LancesCampaignBehavior());
                campaignGameStarter.AddBehavior(new AskForVolunteersCampaignBehavior());
                campaignGameStarter.AddBehavior(new MercenaryLancesInTavernsCampaignBehavior());
                campaignGameStarter.AddBehavior(new NotablesInCastlesBehavior());
                campaignGameStarter.AddBehavior(new DisbandedLanceAI());
                campaignGameStarter.RemoveBehavior(campaignGameStarter.CampaignBehaviors.First(b => b is RecruitmentCampaignBehavior));
                campaignGameStarter.AddBehavior(new AILanceRecruitment());
                campaignGameStarter.AddBehavior(new AIGoBuyFoodBehavior());
                
                campaignGameStarter.AddModel(new LancePartySizeLimitModel(gameStarterObject.GetExistingModel<PartySizeLimitModel>()));
                campaignGameStarter.AddModel(new LanceTavernMercenaryTroopsModel());
            }
        }
        protected override void OnSubModuleLoad()
        {
            harmony.PatchAll();
            LanceTemplateManager.Instance.LoadFromFile();
            DynamicTroopsXmlSaver xmlSaver = new(Path.Combine(PathHelper.OutsideConfigPath, "dynamic_troops.xml"));
            xmlSaver.CreateCharacterXmlIfNeeded();
            xmlSaver.LoadAndMarkDynamic();
            CustomSettingsBootstrap.Initialize();
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
