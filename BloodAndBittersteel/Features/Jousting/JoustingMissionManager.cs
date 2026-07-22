using SandBox;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.View;
using SandBox.View.Missions;
using SandBox.View.Missions.Sound.Components;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using SandBox.ViewModelCollection;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions;
using static TaleWorlds.MountAndBlade.Mission;
using SandBox.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.Engine;

namespace BloodAndBittersteel.Features.Jousting
{
    [MissionManager]
    public static class JoustingMissionManager
    {
        [MissionMethod]
        public static Mission OpenJoustingFightMission(string scene, JoustTournamentGame tournamentGame, Settlement settlement, CultureObject culture, bool isPlayerParticipating)
        {
            JoustFightMissionController joustFightMissionController = new JoustFightMissionController(culture);
            return MissionState.OpenNew("JoustFight", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false, DecalAtlasGroup.Town),
            (missionController) => new MissionBehavior[]
            {
                new CampaignMissionComponent(),
                new EquipmentControllerLeaveLogic(),
                joustFightMissionController,
                new JoustTournamentBehavior(tournamentGame, settlement, joustFightMissionController, isPlayerParticipating),
                new AgentVictoryLogic(),
                new MissionAgentPanicHandler(),
                new AgentHumanAILogic(),
                new ArenaAgentStateDeciderLogic(),
                new MissionHardBorderPlacer(),
                new MissionBoundaryPlacer(),
                new MissionOptionsComponent(),
                new HighlightsController(),
                new SandboxHighlightsController()
            }, true, true);
        }
    }

    [ViewCreatorModule]
    public class JoustFightViewCreatorModule
    {
        [ViewMethod("JoustFight")]
        public static MissionView[] OpenJoustFightMission(Mission mission)
        {
            return new List<MissionView>
                {
                    new MissionCampaignView(),
                    new MissionConversationCameraView(),
                    ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
                    ViewCreator.CreateOptionsUIHandler(),
                    ViewCreator.CreateMissionMainAgentEquipDropView(mission),
                    SandBoxViewCreator.CreateMissionTournamentView(),
                    new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.6f),
                    ViewCreator.CreateMissionAgentStatusUIHandler(mission),
                    ViewCreator.CreateMissionMainAgentEquipmentController(mission),
                    ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
                    ViewCreator.CreateMissionAgentLockVisualizerView(mission),
                    ViewCreator.CreateMissionSpectatorControlView(mission),
                    new MusicTournamentMissionView(),
                    new MissionSingleplayerViewHandler(),
                    ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
                    ViewCreator.CreateMissionAgentLabelUIHandler(mission),
                    new MissionItemContourControllerView(),
                    new MissionCampaignBattleSpectatorView(),
                    ViewCreator.CreatePhotoModeView(),
    
                    //ViewCreator.CreateMissionLeaveView(),
                    //new MissionCameraFadeView()
                }.ToArray();
        }
    }
}
