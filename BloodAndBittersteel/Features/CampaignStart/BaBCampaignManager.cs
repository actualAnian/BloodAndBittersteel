using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem.Load;

namespace BloodAndBittersteel.Features.CampaignStart
{
    public class BaBCampaignManager : MBGameManager
    {
        public bool LoadingSavedGame { get; private set; }
        private readonly SandBoxGameManager.CampaignCreatorDelegate? _campaignCreator;
        private LoadResult? _loadedGameResult;
        public BaBCampaignManager(SandBoxGameManager.CampaignCreatorDelegate campaignCreator)
        {
            LoadingSavedGame = false;
            _campaignCreator = campaignCreator;
        }

        public BaBCampaignManager(LoadResult loadedGameResult)
        {
            LoadingSavedGame = true;
            _loadedGameResult = loadedGameResult;
        }

        public override void OnGameEnd(Game game)
        {
            MBDebug.SetErrorReportScene(null);
            base.OnGameEnd(game);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
        }

        protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
        {
            nextStep = GameManagerLoadingSteps.None;
            switch (gameManagerLoadingStep)
            {
                case GameManagerLoadingSteps.PreInitializeZerothStep:
                    nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
                    return;
                case GameManagerLoadingSteps.FirstInitializeFirstStep:
                    LoadModuleData(LoadingSavedGame);
                    nextStep = GameManagerLoadingSteps.WaitSecondStep;
                    return;
                case GameManagerLoadingSteps.WaitSecondStep:
                    if (!LoadingSavedGame)
                        StartNewGame();
                    nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
                    return;
                case GameManagerLoadingSteps.SecondInitializeThirdState:
                    MBGlobals.InitializeReferences();
                    if (!LoadingSavedGame)
                    {
                        MBDebug.Print("Initializing new game begin...", 0, Debug.DebugColor.White, 17592186044416UL);
                        Campaign campaign = _campaignCreator();
                        Game.CreateGame(campaign, this);
                        campaign.SetLoadingParameters(Campaign.GameLoadingType.NewCampaign);
                        MBDebug.Print("Initializing new game end...", 0, Debug.DebugColor.White, 17592186044416UL);
                    }
                    else
                    {
                        MBDebug.Print("Initializing saved game begin...", 0, Debug.DebugColor.White, 17592186044416UL);
                        ((Campaign)Game.LoadSaveGame(_loadedGameResult, this).GameType).SetLoadingParameters(Campaign.GameLoadingType.SavedCampaign);
                        _loadedGameResult = null;
                        Common.MemoryCleanupGC(false);
                        MBDebug.Print("Initializing saved game end...", 0, Debug.DebugColor.White, 17592186044416UL);
                    }
                    Game.Current.DoLoading();
                    nextStep = GameManagerLoadingSteps.PostInitializeFourthState;
                    return;
                case GameManagerLoadingSteps.PostInitializeFourthState:
                    {
                        bool flag = true;
                        foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.CollectSubModules())
                            flag = (flag && mbsubModuleBase.DoLoading(Game.Current));
                        nextStep = (flag ? GameManagerLoadingSteps.FinishLoadingFifthStep : GameManagerLoadingSteps.PostInitializeFourthState);
                        return;
                    }
                case GameManagerLoadingSteps.FinishLoadingFifthStep:
                    nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.None : GameManagerLoadingSteps.FinishLoadingFifthStep);
                    return;
                default:
                    return;
            }
        }

        private void LaunchSandboxCharacterCreation()
        {
            CharacterCreationState gameState = Game.Current.GameStateManager.CreateState<CharacterCreationState>();
            Game.Current.GameStateManager.CleanAndPushState(gameState, 0);
        }

        public override void OnLoadFinished()
        {
            if (!LoadingSavedGame)
            {
                MBDebug.Print("OnLoadFinished DevelopmentMode", 0, Debug.DebugColor.White, 17592186044416UL);
                MBDebug.Print("Launching Sandbox Character Creation", 0, Debug.DebugColor.White, 17592186044416UL);
                LaunchSandboxCharacterCreation();
            }
            else
            {
                MBDebug.Print("Loading Save Game", 0, Debug.DebugColor.White, 17592186044416UL);
                Game.Current.GameStateManager.OnSavedGameLoadFinished();
                Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<MapState>(), 0);
                MapState? mapState = Game.Current.GameStateManager.ActiveState as MapState;
                string text = mapState?.GameMenuId!;
                if (!string.IsNullOrEmpty(text))
                {
                    if (Campaign.Current.GameMenuManager.GetGameMenu(text) != null)
                    {
                        PlayerEncounter playerEncounter = PlayerEncounter.Current;
                        playerEncounter?.OnLoad();
                        Campaign.Current.GameMenuManager.SetNextMenu(text);
                    }
                    else
                        PlayerEncounter.Finish(true);
                }
                PartyBase.MainParty.SetVisualAsDirty();
                Campaign.Current.CampaignInformationManager.OnGameLoaded();
                foreach (Settlement settlement in Settlement.All)
                    settlement.Party.SetLevelMaskIsDirty();
                CampaignEventDispatcher.Instance.OnGameLoadFinished();
                mapState?.OnLoadingFinished();
            }
            IsLoaded = true;
        }

        public override void OnAfterCampaignStart(Game game) {}
    }
}
