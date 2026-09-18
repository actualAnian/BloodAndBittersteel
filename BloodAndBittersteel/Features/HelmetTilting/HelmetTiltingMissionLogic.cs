using BloodAndBittersteel.MCM;
using System;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.HelmetTilting
{
    public class HelmetTiltingMissionLogic : MissionLogic
    {
        public override void OnMissionTick(float dt)
        {
            if (Input.IsKeyPressed(BaBSettings.Instance.HelmetTilting))
            {
                if (Agent.Main == null) return;
                foreach (EquipmentIndex eqIndex in Enum.GetValues(typeof(EquipmentIndex)))
                {
                    if ((int)eqIndex >= 12 || (int)eqIndex < 0) continue;
                    if (Agent.Main.SpawnEquipment[eqIndex].IsEmpty) continue;
                    foreach (var swap in HelmetSwapManager.Instance.Swaps)
                    {
                        string currentId = Agent.Main.SpawnEquipment[eqIndex].Item.StringId;
                        if (currentId == swap.VisorOpenedItemId)
                            SwapItem(eqIndex, swap.VisorClosedItemId, "act_visor_close");
                        else if (currentId == swap.VisorClosedItemId)
                            SwapItem(eqIndex, swap.VisorOpenedItemId, "act_visor_open");
                    }
                }
            }
        }
        private async void ChangePlayerHelmetWithDelay(EquipmentIndex index, ItemObject newItem, int delayInMiliseconds = 700)
        {
            await Task.Delay(delayInMiliseconds);
            var newEquipment = new Equipment(Agent.Main.SpawnEquipment);
            for (int i = 0; i < (int)EquipmentIndex.ArmorItemBeginSlot; i++)
            {
                if (!Agent.Main.Equipment[i].IsEmpty)
                    newEquipment[i] = new EquipmentElement(Agent.Main.Equipment[i].Item);
            }
            newEquipment[index] = new EquipmentElement(newItem);
            Agent.Main.RefreshCharacterEquipmentWithoutStoppingAnimation(newEquipment);
        }
        private void SwapItem(EquipmentIndex index, string newItemId, string animationAction)
        {
            var newItem = MBObjectManager.Instance.GetObject<ItemObject>(newItemId);
            Agent.Main.SetActionChannel(0, ActionIndexCache.Create(animationAction), true);
            ChangePlayerHelmetWithDelay(index, newItem);
        }
    }
}