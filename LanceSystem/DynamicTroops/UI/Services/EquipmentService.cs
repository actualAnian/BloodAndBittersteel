using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using LanceSystem.DynamicTroops.UI.ItemSelection;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;

namespace LanceSystem.DynamicTroops.TroopCreation.Services
{
    public class EquipmentService
    {
        readonly CharacterObject _character;
        readonly Action _refresh;
        public List<int> UpdateSlots { get; private set; } = new();
        public EquipmentService(CharacterObject character, Action refresh)
        {
            _character = character;
            _refresh = refresh;
        }
        public void SelectItem(string slotKey)
        {
            List<Equipment> battleEquipments = _character.BattleEquipments.ToList();
            List<InquiryElement> elements = BuildItemSetElements(battleEquipments, slotKey);
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select variations sets to change", "", elements, true, 1, battleEquipments.Count, "Continue", null, args =>
            {
                if (args == null || !args.Any()) return;
                InformationManager.HideInquiry();
                UpdateSlots = args.Select(a => (int)a.Identifier).ToList();
                OpenItemSelector(slotKey);
            }, null, "", false), false, false);
        }
        List<InquiryElement> BuildItemSetElements(List<Equipment> equipments, string slotKey)
        {
            List<InquiryElement> list = new();
            EquipmentIndex index = ToItemSlot(slotKey);
            for (int i = 0; i < equipments.Count; i++)
            {
                EquipmentElement element = equipments[i][index];
                string name = element.Item != null ? element.Item.Name.ToString() : "Empty";
                ImageIdentifier identifier = element.Item != null ? new ItemImageIdentifier(element.Item) : null;
                list.Add(new InquiryElement(i, (i + 1) + " : " + name, identifier));
            }
            return list;
        }
        void OpenItemSelector(string slotKey)
        {
            EquipmentIndex slot = ToItemSlot(slotKey);
            List<ItemObject> items = new();
            foreach (ItemObject item in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
            {
                if (!GetRequiredItemType(slot).Contains(item.Type)) continue;
                if (item.IsCraftedByPlayer) continue;
                items.Add(item);
            }
            var controller = new ObjectSelectorController<ItemObject>(items, _character, slotKey, item => FinalizeItem(slot, item), item => new ObjectCardVM(item, _character, slotKey, i => FinalizeItem(slot, i)), FilterFactory.CreateEquipmentFilters());
            controller.Open();
        }
        public void FinalizeItem(EquipmentIndex equipmentIndex, ItemObject item)
        {
            foreach (int slot in UpdateSlots) ChangeUnitEquipment((int)equipmentIndex, item, slot);
            _refresh();
            SetDefaultGroup();
        }
        void ChangeUnitEquipment(int slot, ItemObject item, int set, bool isCivilian = false)
        {
            List<Equipment> civilian = _character.CivilianEquipments.ToList();
            List<Equipment> battle = _character.BattleEquipments.ToList();
            EquipmentElement value = item == null ? default : new EquipmentElement(item, null, null, false);
            if (isCivilian) civilian[set][slot] = value;
            else battle[set][slot] = value;
            List<Equipment> combined = new();
            combined.AddRange(battle);
            combined.AddRange(civilian);
            UpdateSelectedUnitEquipment(combined);
        }
        void UpdateSelectedUnitEquipment(List<Equipment> equipments)
        {
            MBEquipmentRoster roster = new();
            ((FieldInfo)GetInstanceField<MBEquipmentRoster>(roster, "_equipments")).SetValue(roster, new MBList<Equipment>(equipments));
            ((FieldInfo)GetInstanceField<BasicCharacterObject>(_character, "_equipmentRoster")).SetValue(_character, roster);
            _character.InitializeEquipmentsOnLoad(_character);
        }
        object GetInstanceField<T>(T instance, string fieldName)
        {
            return typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }
        public void SetDefaultGroup()
        {
            bool hasMount = _character.Equipment[EquipmentIndex.Horse].Item != null;
            bool isRanged = IsRanged();
            FormationClass formation = FormationClass.Infantry;
            if (isRanged && hasMount) formation = FormationClass.Cavalry;
            else if (hasMount) formation = FormationClass.Cavalry;
            else if (isRanged) formation = FormationClass.Ranged;
            typeof(CharacterObject).GetProperty("DefaultFormationClass").SetValue(_character, formation, null);
            typeof(CharacterObject).GetProperty("DefaultFormationGroup")?.SetValue(_character, formation, null);
        }
        bool IsRanged()
        {
            for (int i = 0; i < 4; i++)
            {
                EquipmentElement element = _character.Equipment[(EquipmentIndex)i];
                if (element.Item != null && (element.Item.Type == ItemObject.ItemTypeEnum.Bow || element.Item.Type == ItemObject.ItemTypeEnum.Crossbow)) return true;
            }
            return false;
        }
        public static EquipmentIndex ToItemSlot(string equipment)
        {
            return equipment switch { "Wep0" => EquipmentIndex.Weapon0, "Wep1" => EquipmentIndex.Weapon1, "Wep2" => EquipmentIndex.Weapon2, "Wep3" => EquipmentIndex.Weapon3, "Head" => EquipmentIndex.Head, "Cape" => EquipmentIndex.Cape, "Body" => EquipmentIndex.Body, "Gloves" => EquipmentIndex.Gloves, "Leg" => EquipmentIndex.Leg, "Horse" => EquipmentIndex.Horse, "Harness" => EquipmentIndex.HorseHarness, _ => (EquipmentIndex)(-1) };
        }
        static List<ItemObject.ItemTypeEnum> GetRequiredItemType(EquipmentIndex index)
        {
            if (index == EquipmentIndex.Weapon0 || index == EquipmentIndex.Weapon1 || index == EquipmentIndex.Weapon2 || index == EquipmentIndex.Weapon3) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.Arrows, ItemObject.ItemTypeEnum.Bolts, ItemObject.ItemTypeEnum.Bow, ItemObject.ItemTypeEnum.Bullets, ItemObject.ItemTypeEnum.Crossbow, ItemObject.ItemTypeEnum.Musket, ItemObject.ItemTypeEnum.OneHandedWeapon, ItemObject.ItemTypeEnum.Pistol, ItemObject.ItemTypeEnum.Polearm, ItemObject.ItemTypeEnum.Shield, ItemObject.ItemTypeEnum.Thrown, ItemObject.ItemTypeEnum.TwoHandedWeapon };
            if (index == EquipmentIndex.Head) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.HeadArmor };
            if (index == EquipmentIndex.Cape) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.Cape };
            if (index == EquipmentIndex.Body) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.BodyArmor };
            if (index == EquipmentIndex.Gloves) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.HandArmor };
            if (index == EquipmentIndex.Leg) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.LegArmor };
            if (index == EquipmentIndex.Horse) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.Horse };
            if (index == EquipmentIndex.HorseHarness) return new List<ItemObject.ItemTypeEnum> { ItemObject.ItemTypeEnum.HorseHarness };
            return new List<ItemObject.ItemTypeEnum>();
        }
    }
}
