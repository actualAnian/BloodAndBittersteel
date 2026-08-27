using LanceSystem.DynamicTroops.TroopCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
namespace LanceSystem.DynamicTroops.TroopCreation.Services
{
    public class UpgradeService
    {
        readonly CharacterObject _character;
        readonly System.Action _refresh;
        public UpgradeService(CharacterObject character, System.Action refresh)
        {
            _character = character;
            _refresh = refresh;
        }
        public void AddUpgrade()
        {
            _refresh();
            List<CharacterObject> upgrades = _character.UpgradeTargets != null ? _character.UpgradeTargets.ToList() : new();
            if (upgrades.Count > 1) return;
            CharacterObject candidate = TaleWorlds.ObjectSystem.MBObjectManager.Instance.GetObject<CharacterObject>(_character.StringId + "0");
            if (candidate != null && !upgrades.Contains(candidate))
            {
                CopyCharacter(_character, candidate);
                upgrades.Add(candidate);
            }
            else
            {
                candidate = TaleWorlds.ObjectSystem.MBObjectManager.Instance.GetObject<CharacterObject>(_character.StringId + "1");
                if (candidate != null && !upgrades.Contains(candidate))
                {
                    CopyCharacter(_character, candidate);
                    upgrades.Add(candidate);
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage("This unit is max tier"));
                    return;
                }
            }
            typeof(CharacterObject).GetProperty("UpgradeTargets").SetValue(_character, upgrades.ToArray(), null);
            _refresh();
            new EquipmentService(_character, _refresh).SetDefaultGroup();
        }
        public void RemoveUpgrade(CharacterObject upgrade)
        {
            List<CharacterObject> upgrades = _character.UpgradeTargets.ToList();
            upgrades.Remove(upgrade);
            typeof(CharacterObject).GetProperty("UpgradeTargets").SetValue(_character, upgrades.Count > 0 ? upgrades.ToArray() : System.Array.Empty<CharacterObject>(), null);
            _refresh();
            new EquipmentService(_character, _refresh).SetDefaultGroup();
        }
        public void CopyTemplate()
        {
            List<InquiryElement> elements = new();
            foreach (CharacterObject candidate in Campaign.Current.Characters)
            {
                if (!IsValidTemplateCandidate(candidate)) continue;
                elements.Add(new InquiryElement(candidate, candidate.Name.ToString(), new CharacterImageIdentifier(CharacterCode.CreateFrom(candidate)), true, BuildHint(candidate)));
            }
            elements.Sort(CompareByName);
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select template to copy from", "", elements, true, 1, 1, "Continue", null, args =>
            {
                if (args == null || !args.Any()) return;
                InformationManager.HideInquiry();
                CharacterObject template = args.Select(e => e.Identifier as CharacterObject).First();
                CopyCharacter(template, _character);
                _refresh();
                new EquipmentService(_character, _refresh).SetDefaultGroup();
            }, null, "", false), false, false);
        }
        bool IsValidTemplateCandidate(CharacterObject candidate)
        {
            if (candidate == null || candidate.IsTemplate || candidate.HiddenInEncyclopedia || candidate.HeroObject != null) return false;
            if (candidate.Tier > _character.Tier && !TroopEditorController.DisableGearRestriction) return false;
            int occupation = (int)candidate.Occupation;
            return occupation == 7 || occupation == 2 || occupation == 15 || occupation == 27 || occupation == 30;
        }
        string BuildHint(CharacterObject character)
        {
            return "level : " + character.Level + "\n" + "tier : " + character.Tier + "\n" + "culture : " + character.Culture.ToString() + "\n" + "one handed : " + character.GetSkillValue(DefaultSkills.OneHanded) + "\n" + "two handed : " + character.GetSkillValue(DefaultSkills.TwoHanded) + "\n" + "polearm : " + character.GetSkillValue(DefaultSkills.Polearm) + "\n" + "bow : " + character.GetSkillValue(DefaultSkills.Bow) + "\n" + "crossbow : " + character.GetSkillValue(DefaultSkills.Crossbow) + "\n" + "throwing : " + character.GetSkillValue(DefaultSkills.Throwing) + "\n" + "riding : " + character.GetSkillValue(DefaultSkills.Riding) + "\n" + "athletics : " + character.GetSkillValue(DefaultSkills.Athletics) + "\n";
        }
        public static void CopyCharacter(CharacterObject original, CharacterObject target)
        {
            CopyEquipment(original, target);
            CopySkills(original, target);
            target.IsFemale = original.IsFemale;
            typeof(BasicCharacterObject).GetProperty("Culture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(target, original.Culture);
            typeof(CharacterObject).GetProperty("BodyPropertyRange")?.SetValue(target, original.BodyPropertyRange, null);
        }
        static void CopyEquipment(CharacterObject original, CharacterObject target)
        {
            EquipmentIndex[] indices = new[] { EquipmentIndex.Weapon0, EquipmentIndex.Weapon1, EquipmentIndex.Weapon2, EquipmentIndex.Weapon3, EquipmentIndex.Head, EquipmentIndex.Body, EquipmentIndex.Leg, EquipmentIndex.Gloves, EquipmentIndex.Cape, EquipmentIndex.Horse, EquipmentIndex.HorseHarness };
            foreach (EquipmentIndex index in indices)
            {
                for (int set = 0; set < target.BattleEquipments.Count(); set++)
                {
                    EquipmentElement element = original.BattleEquipments.ToArray()[0][index];
                    ChangeUnitEquipment(target, (int)index, element.Item, set);
                }
            }
        }
        static void ChangeUnitEquipment(CharacterObject character, int slot, ItemObject item, int set)
        {
            List<Equipment> civilian = character.CivilianEquipments.ToList();
            List<Equipment> battle = character.BattleEquipments.ToList();
            EquipmentElement value = item == null ? default : new EquipmentElement(item, null, null, false);
            battle[set][slot] = value;
            List<Equipment> combined = new();
            combined.AddRange(battle);
            combined.AddRange(civilian);
            UpdateSelectedUnitEquipment(character, combined);
        }
        static void UpdateSelectedUnitEquipment(CharacterObject character, List<Equipment> equipments)
        {
            MBEquipmentRoster roster = new();
            ((FieldInfo)GetInstanceField<MBEquipmentRoster>(roster, "_equipments")).SetValue(roster, new MBList<Equipment>(equipments));
            ((FieldInfo)GetInstanceField<BasicCharacterObject>(character, "_equipmentRoster")).SetValue(character, roster);
            character.InitializeEquipmentsOnLoad(character);
        }
        static object GetInstanceField<T>(T instance, string fieldName)
        {
            return typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }
        static void CopySkills(CharacterObject original, CharacterObject target)
        {
            MBCharacterSkills skills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(target.StringId);
            skills.Skills.SetPropertyValue(DefaultSkills.Crossbow, original.GetSkillValue(DefaultSkills.Crossbow));
            skills.Skills.SetPropertyValue(DefaultSkills.Bow, original.GetSkillValue(DefaultSkills.Bow));
            skills.Skills.SetPropertyValue(DefaultSkills.Throwing, original.GetSkillValue(DefaultSkills.Throwing));
            skills.Skills.SetPropertyValue(DefaultSkills.OneHanded, original.GetSkillValue(DefaultSkills.OneHanded));
            skills.Skills.SetPropertyValue(DefaultSkills.TwoHanded, original.GetSkillValue(DefaultSkills.TwoHanded));
            skills.Skills.SetPropertyValue(DefaultSkills.Polearm, original.GetSkillValue(DefaultSkills.Polearm));
            skills.Skills.SetPropertyValue(DefaultSkills.Athletics, original.GetSkillValue(DefaultSkills.Athletics));
            skills.Skills.SetPropertyValue(DefaultSkills.Riding, original.GetSkillValue(DefaultSkills.Riding));
            FieldInfo field = target.GetType().GetField("DefaultCharacterSkills", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field?.SetValue(target, skills);
        }
        static int CompareByName(InquiryElement x, InquiryElement y)
        {
            if (x?.Identifier == null) return -1;
            if (y?.Identifier == null) return 1;
            return string.Compare(((CharacterObject)x.Identifier).Name.ToString(), ((CharacterObject)y.Identifier).Name.ToString());
        }
    }
}

