using BloodAndBittersteel.MCM;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.HelmetTilting
{
    public class HelmetTiltingMissionLogic : MissionLogic
    {
        private readonly static string CanNotSwapText = "{=bab_can_not_tilt_helmet}You need to put your shield away first";
        private bool HasWeaponInLeftHand(Agent agent)
        {
            return agent.WieldedOffhandWeapon.Item != null;
        }
        public override void OnMissionTick(float dt)
        {
            Agent agent = Agent.Main;

            if (agent == null || !Input.IsKeyPressed(BaBSettings.Instance.HelmetTilting))
                return;
            ToggleHelmetVisor(agent);
        }
        private void ToggleHelmetVisor(Agent agent)
        {
            bool isLeftStance = agent.GetIsLeftStance();
            foreach (EquipmentIndex equipmentIndex in Enum.GetValues(typeof(EquipmentIndex)).Cast<EquipmentIndex>().Where(index => (int)index >= 0 && (int)index < 12))
            {
                if (agent.SpawnEquipment[equipmentIndex].IsEmpty)
                    continue;
                string currentItemId = agent.SpawnEquipment[equipmentIndex].Item.StringId;
                foreach (var swap in HelmetSwapManager.Instance.Swaps)
                {
                    if (currentItemId == swap.VisorOpenedItemId)
                    {
                        if (HasWeaponInLeftHand(agent))
                        {
                            MBInformationManager.AddQuickInformation(new(CanNotSwapText));
                            return;
                        }
                        SwapItem(equipmentIndex, swap.VisorClosedItemId, ChooseAnimation(isLeftStance, false));
                        return;
                    }
                    if (currentItemId == swap.VisorClosedItemId)
                    {
                        if (HasWeaponInLeftHand(agent))
                        {
                            MBInformationManager.AddQuickInformation(new(CanNotSwapText));
                            return;
                        }
                        SwapItem(equipmentIndex, swap.VisorOpenedItemId, ChooseAnimation(isLeftStance, true));
                        return;
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
            Agent.Main.RefreshCharacterEquipmentKeepAnimationAndWieldedWeapon(newEquipment);
        }
        private void SwapItem(EquipmentIndex index, string newItemId, string animationAction)
        {
            var newItem = MBObjectManager.Instance.GetObject<ItemObject>(newItemId);
            Agent.Main.SetActionChannel(0, ActionIndexCache.Create(animationAction), true);
            ChangePlayerHelmetWithDelay(index, newItem);
        }
        private string ChooseAnimation(bool isLeftStance, bool isOpeningVisor)
        {
            switch (isLeftStance, isOpeningVisor)
            {
                case (true, true):
                    return "act_visor_open_leftstance";
                case (true, false):
                    return "act_visor_close_leftstance";
                case (false, true):
                    return "act_visor_open";
                case (false, false):
                    return "act_visor_close";
            }
        }
    }
}