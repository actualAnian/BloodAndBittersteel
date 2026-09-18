using System.Reflection;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BloodAndBittersteel.Features.HelmetTilting
{
    public static class AgentExtensions
    {
        private static readonly PropertyInfo SpawnEquipmentProperty = typeof(Agent).GetProperty("SpawnEquipment", BindingFlags.Public | BindingFlags.Instance)!;

        public static void RefreshCharacterEquipmentWithoutStoppingAnimation(this Agent agent, Equipment newSpawnEquipment)
        {
            // forked from Agent.UpdateSpawnEquipmentAndRefreshVisuals
            SpawnEquipmentProperty.SetValue(agent, newSpawnEquipment);
            if (GameNetwork.IsServerOrRecorder)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new SynchronizeAgentSpawnEquipment(agent.Index, agent.SpawnEquipment));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
            }
            agent.EquipItemsFromSpawnEquipment(true, true, false, 0);
            agent.CheckEquipmentForCapeClothSimulationStateChange();
            agent.UpdateAgentProperties();
            //agent.PreloadForRendering(); // 1.5.1 this method stops any current player animation, can not be used
        }
    }
}
