using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.Core.ViewModelCollection.Information.TooltipProperty;

namespace LanceSystem.DynamicTroops.UI.ItemSelection
{
    public class ItemCardVM : CardVM
    {
        readonly ItemObject _item;
        readonly CharacterObject _troop;

        public ItemCardVM(ItemObject item, CharacterObject troop, Action<ItemObject> apply, Action? close = null)
            : base(() => apply(item), close)
        {
            _item = item;
            _troop = troop;
            if (_item == null)
            {
                CardName = "Empty";
                return;
            }
            CardName = _item.Name.ToString();
            Image = new ItemImageIdentifierVM(_item, "");
            InitializeFlags();
            InitializeProperties();
        }

        void InitializeFlags()
        {
            if (_item == null) return;
            if (_item.WeaponComponent?.Item != null)
                AddWeaponItemFlags(ItemFlagList, _item.WeaponComponent.Item.GetWeaponWithUsageIndex(0));
            AddGeneralItemFlags(ItemFlagList, _item);
            if (_item.HorseComponent != null)
                AddHorseItemFlags(ItemFlagList, _item);
        }

        void InitializeProperties()
        {
            if (_item != null)
                AddCoinProperty();
            BuildCultureProperty();
            if (_item?.RelevantSkill != null && _item.Difficulty > 0)
                AddSkillRequirement();
            BuildHorseProperties();
            BuildWeaponProperties();
            BuildArmorProperties();
        }

        void AddCoinProperty()
        {
            string coinValue = _item.Value + "<img src=\"General\\Icons\\Coin@2x\" extend=\"8\"/>";
            CreateColoredProperty(ObjectProperties, "", coinValue, UIColors.Gold, 1);
        }

        void BuildCultureProperty()
        {
            if (_item?.Culture?.Name != null)
            {
                CreateColoredProperty(ObjectProperties, "Culture: ", _item.Culture.Name.ToString(), Color.FromUint(_item.Culture.Color));
                return;
            }
            CreateColoredProperty(ObjectProperties, "Culture: ", "No Culture", UIColors.Gold);
        }

        void BuildHorseProperties()
        {
            if (_item?.HorseComponent == null) return;
            var element = new EquipmentElement(_item);
            AddTextProperty(GameTexts.FindText("str_inventory_type_" + (int)_item.Type), new TextObject("{=08abd5af7774d311cadc3ed900b47754}Type: "));
            AddIntProperty(new TextObject("{=mountTier}Mount Tier: "), (int)_item.Tier + 1);
            AddIntProperty(new TextObject("{=c7638a0869219ae845de0f660fd57a9d}Charge Damage: "), element.GetModifiedMountCharge(in EquipmentElement.Invalid));
            AddIntProperty(GameTexts.FindText("str_mount_speed"), element.GetModifiedMountSpeed(in EquipmentElement.Invalid));
            AddIntProperty(new TextObject("{=3025020b83b218707499f0de3135ed0a}Maneuver: "), element.GetModifiedMountManeuver(in EquipmentElement.Invalid));
            AddIntProperty(GameTexts.FindText("str_hit_points"), element.GetModifiedMountHitPoints());
            if (_item.HasHorseComponent && _item.HorseComponent.IsMount)
                AddTextProperty(_item.ItemCategory.GetName(), new TextObject("{=9sxECG6e}Mount Type: "));
        }

        void BuildWeaponProperties()
        {
            if (_item?.WeaponComponent == null) return;
            var weapon = _item.WeaponComponent.Item.GetWeaponWithUsageIndex(0);
            AddTextProperty(GameTexts.FindText("str_inventory_weapon", ((int)weapon.WeaponClass).ToString()), new TextObject("{=8cad4a279770f269c4bb0dc7a357ee1e}Class: "));
            if (_item.BannerComponent == null)
                AddIntProperty(new TextObject("{=weaponTier}Weapon Tier: "), (int)_item.Tier + 1);
            var type = WeaponComponentData.GetItemTypeFromWeaponClass(weapon.WeaponClass);
            BuildWeaponTypeProperties(weapon, type);
        }

        void BuildWeaponTypeProperties(WeaponComponentData weapon, ItemObject.ItemTypeEnum type)
        {
            switch (type)
            {
                case ItemObject.ItemTypeEnum.OneHandedWeapon:
                case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                case ItemObject.ItemTypeEnum.Polearm:
                    BuildMeleeProperties(weapon);
                    break;
                case ItemObject.ItemTypeEnum.Thrown:
                    BuildThrownProperties(weapon);
                    break;
                case ItemObject.ItemTypeEnum.Shield:
                    BuildShieldProperties(weapon);
                    break;
                case ItemObject.ItemTypeEnum.Bow:
                case ItemObject.ItemTypeEnum.Crossbow:
                    BuildRangedProperties(weapon, type);
                    break;
                default:
                    if (weapon.IsAmmo)
                        BuildAmmoProperties(weapon, type);
                    break;
            }
        }

        void BuildMeleeProperties(WeaponComponentData weapon)
        {
            var element = new EquipmentElement(_item);
            if (weapon.SwingDamageType != DamageTypes.Invalid)
            {
                AddIntProperty(new TextObject("{=345a87fcc69f626ae3916939ef2fc135}Swing Speed: "), element.GetModifiedSwingSpeedForUsage(0));
                AddTextProperty(ItemHelper.GetSwingDamageText(weapon, element.ItemModifier), GameTexts.FindText("str_swing_damage"));
            }
            if (weapon.ThrustDamageType != DamageTypes.Invalid)
            {
                AddIntProperty(GameTexts.FindText("str_thrust_speed"), element.GetModifiedThrustSpeedForUsage(0));
                AddTextProperty(ItemHelper.GetThrustDamageText(weapon, element.ItemModifier), GameTexts.FindText("str_thrust_damage"));
            }
            AddIntProperty(new TextObject("{=c6e4c8588ca9e42f6e1b47b11f0f367b}Length: "), weapon.WeaponLength);
            AddIntProperty(new TextObject("{=ca8b1e8956057b831dfc665f54bae4b0}Handling: "), element.GetModifiedHandlingForUsage(0));
        }

        void BuildThrownProperties(WeaponComponentData weapon)
        {
            var element = new EquipmentElement(_item);
            AddIntProperty(new TextObject("{=5fa36d2798479803b4518a64beb4d732}Weapon Length: "), weapon.WeaponLength);
            AddTextProperty(ItemHelper.GetMissileDamageText(weapon, element.ItemModifier), new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: "));
            AddIntProperty(GameTexts.FindText("str_missile_speed"), element.GetModifiedMissileSpeedForUsage(0));
            AddIntProperty(new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: "), weapon.Accuracy);
            AddIntProperty(new TextObject("{=05fdfc6e238429753ef282f2ce97c1f8}Stack Amount: "), element.GetModifiedStackCountForUsage(0));
        }

        void BuildShieldProperties(WeaponComponentData weapon)
        {
            var element = new EquipmentElement(_item);
            AddIntProperty(new TextObject("{=74dc1908cb0b990e80fb977b5a0ef10d}Speed: "), element.GetModifiedSwingSpeedForUsage(0));
            AddIntProperty(GameTexts.FindText("str_hit_points"), element.GetModifiedMaximumHitPointsForUsage(0));
        }

        void BuildRangedProperties(WeaponComponentData weapon, ItemObject.ItemTypeEnum type)
        {
            var element = new EquipmentElement(_item);
            AddIntProperty(new TextObject("{=74dc1908cb0b990e80fb977b5a0ef10d}Speed: "), element.GetModifiedSwingSpeedForUsage(0));
            AddTextProperty(ItemHelper.GetThrustDamageText(weapon, element.ItemModifier), new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: "));
            AddIntProperty(new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: "), weapon.Accuracy);
            AddIntProperty(GameTexts.FindText("str_missile_speed"), element.GetModifiedMissileSpeedForUsage(0));
            if (type == ItemObject.ItemTypeEnum.Crossbow)
                AddIntProperty(new TextObject("{=6adabc1f82216992571c3e22abc164d7}Ammo Limit: "), weapon.MaxDataValue);
        }

        void BuildAmmoProperties(WeaponComponentData weapon, ItemObject.ItemTypeEnum type)
        {
            var element = new EquipmentElement(_item);
            if (type != ItemObject.ItemTypeEnum.Arrows && type != ItemObject.ItemTypeEnum.Bolts)
                AddIntProperty(new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: "), weapon.Accuracy);
            AddTextProperty(ItemHelper.GetThrustDamageText(weapon, element.ItemModifier), new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: "));
            AddIntProperty(new TextObject("{=05fdfc6e238429753ef282f2ce97c1f8}Stack Amount: "), element.GetModifiedStackCountForUsage(0));
        }

        void BuildArmorProperties()
        {
            if (_item?.ArmorComponent == null) return;
            var element = new EquipmentElement(_item);
            AddIntProperty(new TextObject("{=armorTier}Armor Tier: "), (int)_item.Tier + 1);
            AddTextProperty(GameTexts.FindText("str_inventory_type_" + (int)_item.Type), new TextObject("{=08abd5af7774d311cadc3ed900b47754}Type: "));
            if (element.GetModifiedHeadArmor() != 0)
                AddTextProperty(element.GetModifiedHeadArmor().ToString(), GameTexts.FindText("str_head_armor"));
            if (_item.ArmorComponent.BodyArmor != 0)
                BuildBodyArmorProperty(element);
            if (element.GetModifiedLegArmor() != 0)
                AddTextProperty(element.GetModifiedLegArmor().ToString(), GameTexts.FindText("str_leg_armor"));
            if (element.GetModifiedArmArmor() != 0)
                AddTextProperty(element.GetModifiedArmArmor().ToString(), new TextObject("{=cf61cce254c7dca65be9bebac7fb9bf5}Arm Armor: "));
        }

        void BuildBodyArmorProperty(EquipmentElement element)
        {
            if (GetItemTypeWithItemObject(_item) == EquipmentIndex.Horse)
                AddTextProperty(element.GetModifiedMountBodyArmor().ToString(), new TextObject("{=305cf7f98458b22e9af72b60a131714f}Horse Armor: "));
            else
                AddTextProperty(element.GetModifiedBodyArmor().ToString(), GameTexts.FindText("str_body_armor"));
        }

        void AddSkillRequirement()
        {
            string skillName = _item.RelevantSkill.Name.ToString();
            string value = skillName + " " + _item.Difficulty;
            bool meetsRequirement = _troop.GetSkillValue(_item.RelevantSkill) >= _item.Difficulty;
            var color = meetsRequirement ? UIColors.PositiveIndicator : UIColors.NegativeIndicator;
            CreateColoredProperty(ObjectProperties, new TextObject("{=154a34f8caccfc833238cc89d38861e8}Requires: ").ToString(), value, color);
        }

        EquipmentIndex GetItemTypeWithItemObject(ItemObject item)
        {
            if (item == null) return (EquipmentIndex)(-1);
            switch (item.Type)
            {
                case ItemObject.ItemTypeEnum.HorseHarness: return EquipmentIndex.HorseHarness;
                case ItemObject.ItemTypeEnum.OneHandedWeapon:
                case ItemObject.ItemTypeEnum.TwoHandedWeapon:
                case ItemObject.ItemTypeEnum.Polearm:
                case ItemObject.ItemTypeEnum.Shield:
                case ItemObject.ItemTypeEnum.Bow:
                case ItemObject.ItemTypeEnum.Crossbow:
                case ItemObject.ItemTypeEnum.Arrows:
                case ItemObject.ItemTypeEnum.Bolts:
                case ItemObject.ItemTypeEnum.Thrown:
                    return EquipmentIndex.Weapon0;
                case ItemObject.ItemTypeEnum.BodyArmor: return EquipmentIndex.Body;
                case ItemObject.ItemTypeEnum.HeadArmor: return EquipmentIndex.Head;
                case ItemObject.ItemTypeEnum.LegArmor: return EquipmentIndex.Leg;
                case ItemObject.ItemTypeEnum.HandArmor: return EquipmentIndex.Gloves;
                case ItemObject.ItemTypeEnum.Pistol: return (EquipmentIndex)10;
                case ItemObject.ItemTypeEnum.Musket: return (EquipmentIndex)11;
                case ItemObject.ItemTypeEnum.Banner: return (EquipmentIndex)9;
                case ItemObject.ItemTypeEnum.Horse: return EquipmentIndex.Horse;
                default:
                    if (item.WeaponComponent != null) return EquipmentIndex.Weapon0;
                    return (EquipmentIndex)(-1);
            }
        }

        void AddIntProperty(TextObject description, int value)
        {
            CreateColoredProperty(ObjectProperties, description.ToString(), value.ToString(), Colors.White);
        }

        void AddTextProperty(object value, TextObject description)
        {
            CreateProperty(ObjectProperties, description.ToString(), value.ToString());
        }

        void AddWeaponItemFlags(MBBindingList<ItemFlagVM> list, WeaponComponentData weapon)
        {
            if (weapon == null) return;
            var flags = MBItem.GetItemUsageSetFlags(weapon.ItemUsage ?? "");
            foreach (var details in CampaignUIHelper.GetFlagDetailsForWeapon(weapon, flags, null))
                list.Add(new ItemFlagVM(details.Item1, details.Item2));
        }

        void AddGeneralItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item)
        {
            if (item.IsUniqueItem)
                list.Add(new ItemFlagVM("GeneralFlagIcons\\unique", GameTexts.FindText("str_inventory_flag_unique")));
            if (item.IsCivilian)
                list.Add(new ItemFlagVM("GeneralFlagIcons\\civillian", GameTexts.FindText("str_inventory_flag_civillian")));
            if (item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByFemale))
                list.Add(new ItemFlagVM("GeneralFlagIcons\\male_only", GameTexts.FindText("str_inventory_flag_male_only")));
            if (item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByMale))
                list.Add(new ItemFlagVM("GeneralFlagIcons\\female_only", GameTexts.FindText("str_inventory_flag_female_only")));
        }

        void AddHorseItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item)
        {
            if (item.HorseComponent.IsLiveStock) return;
            if (item.ItemCategory == DefaultItemCategories.PackAnimal)
                list.Add(new ItemFlagVM("MountFlagIcons\\weight_carrying_mount", GameTexts.FindText("str_inventory_flag_carrying_mount")));
            else
                list.Add(new ItemFlagVM("MountFlagIcons\\speed_mount", GameTexts.FindText("str_inventory_flag_speed_mount")));
        }

        ItemMenuTooltipPropertyVM CreateColoredProperty(MBBindingList<ItemMenuTooltipPropertyVM> targetList, string definition, string value, Color color, int textHeight = 0, HintViewModel? hint = null, TooltipPropertyFlags propertyFlags = default)
        {
            if (color == Colors.Black)
                return CreateProperty(targetList, definition, value, textHeight, hint);
            var property = new ItemMenuTooltipPropertyVM(definition, value, textHeight, color, false, hint, propertyFlags);
            targetList.Add(property);
            return property;
        }

        ItemMenuTooltipPropertyVM CreateProperty(MBBindingList<ItemMenuTooltipPropertyVM> targetList, string definition, string value, int textHeight = 0, HintViewModel? hint = null)
        {
            var property = new ItemMenuTooltipPropertyVM(definition, value, textHeight, false, hint);
            targetList.Add(property);
            return property;
        }
    }
}
