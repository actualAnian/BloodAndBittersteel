using System.Reflection;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BloodAndBittersteel.Features.HelmetTilting
{
    public static class AgentExtensions
    {
        private static readonly PropertyInfo SpawnEquipmentProperty = typeof(Agent).GetProperty("SpawnEquipment", BindingFlags.Public | BindingFlags.Instance)!;

        public static void RefreshCharacterEquipmentKeepAnimationAndWieldedWeapon(this Agent agent, Equipment newSpawnEquipment)
        {
            // forked from Agent.UpdateSpawnEquipmentAndRefreshVisuals
            SpawnEquipmentProperty.SetValue(agent, newSpawnEquipment);
            if (GameNetwork.IsServerOrRecorder)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new SynchronizeAgentSpawnEquipment(agent.Index, agent.SpawnEquipment));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
            }
            ItemObject? wieldedItemBeforeEdit = agent.WieldedWeapon.Item;
            agent.EquipItemsFromSpawnEquipment(true, true, false, 0);
            agent.CheckEquipmentForCapeClothSimulationStateChange();
            var slotIndex = EquipmentIndex.None;
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NonWeaponItemBeginSlot; equipmentIndex++)
            {
                if (!agent.Equipment[equipmentIndex].IsEmpty && wieldedItemBeforeEdit == agent.Equipment[equipmentIndex].Item)
                {
                    slotIndex = equipmentIndex;
                    break;
                }
            }
            if (slotIndex != EquipmentIndex.None)
                agent.TryToWieldWeaponInSlot(slotIndex, Agent.WeaponWieldActionType.Instant, true);
            agent.UpdateAgentProperties();
            //agent.PreloadForRendering(); // 1.5.2 this method stops any current player animation, can not be used
        }
    }
}
