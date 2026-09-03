using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LanceSystem.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using LanceSystem.DynamicTroops.UI.ItemSelection;
using static LanceSystem.DynamicTroops.UI.ItemSelection.Filters.FilterFactory;

namespace LanceSystem.DynamicTroops.UI.Services
{
    public class EquipmentService
    {
        static readonly TextObject _selectVariationsTitle = new("{=lance_select_variations}Select variations sets to change");
        static readonly TextObject _selectWeaponText = new("{=lance_select_weapon}Select Weapon");
        static readonly TextObject _selectHelmetText = new("{=lance_select_helmet}Select Helmet");
        static readonly TextObject _selectBodyArmorText = new("{=lance_select_body_armor}Select Body Armor");
        static readonly TextObject _selectGlovesText = new("{=lance_select_gloves}Select Gloves");
        static readonly TextObject _selectLegArmorText = new("{=lance_select_leg_armor}Select Leg Armor");
        static readonly TextObject _selectCapeText = new("{=lance_select_cape}Select Cape");
        static readonly TextObject _selectHorseText = new("{=lance_select_horse}Select Horse");
        static readonly TextObject _selectHorseHarnessText = new("{=lance_select_horse_harness}Select Horse Harness");
        static readonly TextObject _selectItemText = new("{=lance_select_item}Select Item");
        readonly Action _refresh;
        List<Equipment> _battleEquipments;
        FormationClass _defaultGroup;
        public List<int> UpdateSlots { get; private set; } = new();
        public EquipmentService(CharacterObject character, Action refresh)
        {
            _refresh = refresh;
            _battleEquipments = character.BattleEquipments.Select(original => new Equipment(original)).ToList();
            _defaultGroup = ComputeDefaultGroup();
        }
        public List<Equipment> GetBattleEquipments() => _battleEquipments;
        public FormationClass GetDefaultGroup() => _defaultGroup;
        public MBEquipmentRoster BuildRoster()
        {
            List<Equipment> combined = new();
            combined.AddRange(_battleEquipments);
                MBEquipmentRoster roster = new();
            typeof(MBEquipmentRoster).GetField("_equipments", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(roster, new MBList<Equipment>(combined));
            return roster;
        }
        public void SelectItem(string slotKey)
        {
            List<InquiryElement> elements = BuildItemSetElements(_battleEquipments, slotKey);
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(_selectVariationsTitle.ToString(), "", elements, true, 1, _battleEquipments.Count, GameTexts.FindText("str_continue", null).ToString(), null, args =>
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
                string name = element.Item != null ? element.Item.Name.ToString() : UITexts.Empty.ToString();
                var identifier = new ItemImageIdentifier(element.Item);
                MBTextManager.SetTextVariable("SET_NUM", i + 1);
                MBTextManager.SetTextVariable("ITEM_NAME", name);
                list.Add(new InquiryElement(i, new TextObject("{=lance_set_entry}{SET_NUM} : {ITEM_NAME}").ToString(), identifier));
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
            string title = GetSlotTitle(slot);
            List<FilterDefinition<ItemObject>> filters = IsWeaponSlot(slot) ? CreateWeaponFilters() : CreateArmorFilters();
            var controller = new ObjectSelectorController<ItemObject>(title, items, item => FinalizeItem(slot, item), (item, close) => new ItemCardVM(item, null, i => FinalizeItem(slot, i), close), filters, emptyCardFactory: (close) => new ItemCardVM(null, null, i => FinalizeItem(slot, i), close));
            controller.Open();
        }
        static bool IsWeaponSlot(EquipmentIndex slot) => slot == EquipmentIndex.Weapon0 || slot == EquipmentIndex.Weapon1 || slot == EquipmentIndex.Weapon2 || slot == EquipmentIndex.Weapon3;
        static string GetSlotTitle(EquipmentIndex slot)
        {
            return slot switch
            {
                EquipmentIndex.Weapon0 or EquipmentIndex.Weapon1 or EquipmentIndex.Weapon2 or EquipmentIndex.Weapon3 => _selectWeaponText.ToString(),
                EquipmentIndex.Head => _selectHelmetText.ToString(),
                EquipmentIndex.Body => _selectBodyArmorText.ToString(),
                EquipmentIndex.Gloves => _selectGlovesText.ToString(),
                EquipmentIndex.Leg => _selectLegArmorText.ToString(),
                EquipmentIndex.Cape => _selectCapeText.ToString(),
                EquipmentIndex.Horse => _selectHorseText.ToString(),
                EquipmentIndex.HorseHarness => _selectHorseHarnessText.ToString(),
                _ => _selectItemText.ToString(),
            };
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
            if (isRanged && hasMount) return FormationClass.HorseArcher;
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
