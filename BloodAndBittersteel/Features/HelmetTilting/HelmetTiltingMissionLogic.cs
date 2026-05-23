using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.HelmetTilting
{
    public class HelmetTiltingMissionLogic : MissionLogic
    {
        //ItemObject item1 = MBObjectManager.Instance.GetObject<ItemObject>("trailed_desert_helmet");
        //ItemObject item2 = MBObjectManager.Instance.GetObject<ItemObject>("emirs_helmet");
        public override void OnMissionTick(float dt)
        {
            if (Input.IsKeyPressed(BaBSettings.Instance.HelmetTilting.SelectedValue))
            {
                if (Agent.Main == null) return;
                foreach(EquipmentIndex eqIndex in Enum.GetValues(typeof(EquipmentIndex)))
                {
                    if ((int)eqIndex >= 12 || (int)eqIndex < 0) continue;
                    if (Agent.Main.SpawnEquipment[eqIndex].IsEmpty) continue;
                    foreach (var swap in ItemSwapManager.Instance.Swaps)
                    {
                        if (swap.ItemIds.Contains(Agent.Main.SpawnEquipment[eqIndex].Item.StringId))
                        {
                            var swapIndex = 1 + swap.ItemIds.IndexOf(Agent.Main.SpawnEquipment[eqIndex].Item.StringId);
                            if (swapIndex >= swap.ItemIds.Count) swapIndex = 0;
                            var newItemId = swap.ItemIds[swapIndex];
                            SwapItem(Agent.Main, eqIndex, MBObjectManager.Instance.GetObject<ItemObject>(newItemId));
                        }
                    }
                }
            }
        }
        public void SwapItem(Agent agent, EquipmentIndex index, ItemObject newObject)
        {
            var newEquipment = new Equipment(agent.SpawnEquipment);
            newEquipment[index] = new EquipmentElement(newObject);
            Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(newEquipment);
        }
    }
}