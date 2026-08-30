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
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters;

namespace LanceSystem.DynamicTroops.UI.Services
{
    public class EquipmentService
    {
        readonly Action _refresh;
        List<Equipment> _battleEquipments;
        List<Equipment> _civilianEquipments;
        FormationClass _defaultGroup;
        public List<int> UpdateSlots { get; private set; } = new();
        public EquipmentService(CharacterObject character, Action refresh)
        {
            _refresh = refresh;
            _battleEquipments = character.BattleEquipments.Select(DeepCopy).ToList();
            _civilianEquipments = character.CivilianEquipments.Select(DeepCopy).ToList();
            _defaultGroup = ComputeDefaultGroup();
        }
        Equipment DeepCopy(Equipment original)
        {
            Equipment copy = new();
            for (int i = 0; i < Equipment.EquipmentSlotLength; i++)
                copy[(EquipmentIndex)i] = original[(EquipmentIndex)i];
            return copy;
        }
        public List<Equipment> GetBattleEquipments() => _battleEquipments;
        public List<Equipment> GetCivilianEquipments() => _civilianEquipments;
        public FormationClass GetDefaultGroup() => _defaultGroup;
        public MBEquipmentRoster BuildRoster()
        {
            List<Equipment> combined = new();
            combined.AddRange(_battleEquipments);
            combined.AddRange(_civilianEquipments);
            MBEquipmentRoster roster = new();
            typeof(MBEquipmentRoster).GetField("_equipments", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(roster, new MBList<Equipment>(combined));
            return roster;
        }
        public void SelectItem(string slotKey)
        {
            List<InquiryElement> elements = BuildItemSetElements(_battleEquipments, slotKey);
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select variations sets to change", "", elements, true, 1, _battleEquipments.Count, "Continue", null, args =>
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
            var controller = new ObjectSelectorController<ItemObject>(items, item => FinalizeItem(slot, item), (item, close) => new ItemCardVM(item, null, i => FinalizeItem(slot, i), close), FilterFactory.CreateEquipmentFilters());
            controller.Open();
        }
        public void FinalizeItem(EquipmentIndex equipmentIndex, ItemObject item)
        {
            foreach (int slot in UpdateSlots) ApplyItem((int)equipmentIndex, item, slot);
            _refresh();
        }
        void ApplyItem(int slot, ItemObject item, int set)
        {
            EquipmentElement value = item == null ? default : new EquipmentElement(item, null, null, false);
            _battleEquipments[set][slot] = value;
        }
        public void AddSet()
        {
            Equipment lastSet = _battleEquipments.Last();
            Equipment newSet = new();
            EquipmentIndex[] indices = new[] { EquipmentIndex.Weapon0, EquipmentIndex.Weapon1, EquipmentIndex.Weapon2, EquipmentIndex.Weapon3, EquipmentIndex.Head, EquipmentIndex.Body, EquipmentIndex.Leg, EquipmentIndex.Gloves, EquipmentIndex.Cape, EquipmentIndex.Horse, EquipmentIndex.HorseHarness };
            foreach (EquipmentIndex index in indices) newSet[index] = lastSet[index];
            _battleEquipments.Add(newSet);
        }
        public void RemoveSets(List<int> indicesToRemove)
        {
            foreach (int index in indicesToRemove) _battleEquipments.RemoveAt(index);
        }
        public void RecalculateDefaultGroup()
        {
            _defaultGroup = ComputeDefaultGroup();
        }
        FormationClass ComputeDefaultGroup()
        {
            if (_battleEquipments.Count == 0) return FormationClass.Infantry;
            Equipment first = _battleEquipments[0];
            bool hasMount = first[EquipmentIndex.Horse].Item != null;
            bool isRanged = HasRanged(first);
            if (isRanged && hasMount) return FormationClass.Cavalry;
            if (hasMount) return FormationClass.Cavalry;
            if (isRanged) return FormationClass.Ranged;
            return FormationClass.Infantry;
        }
        bool HasRanged(Equipment equipment)
        {
            for (int i = 0; i < 4; i++)
            {
                EquipmentElement element = equipment[(EquipmentIndex)i];
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
