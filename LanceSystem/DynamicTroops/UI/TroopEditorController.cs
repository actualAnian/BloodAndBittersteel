using HarmonyLib;
using LanceSystem.DynamicTroops.UI.ItemSelection;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters;
using LanceSystem.Deserialization;
using LanceSystem.DynamicTroops.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops.UI
{
    public class TroopEditorController
    {
        public static bool DisableGearRestriction;
        public static bool DisableSkillTotalRestriction;
        public CharacterObject PreviewCharacter { get; }
        public TroopEditorVM Vm { get; }
        readonly SkillService _skillService;
        readonly AppearanceService _appearanceService;
        readonly EquipmentService _equipmentService;
        readonly TroopUpgradeService _upgradeService;
        int _level;
        public TroopEditorController(CharacterObject character)
        {
            PreviewCharacter = CharacterObject.CreateFrom(character);
            _level = character.Level;
            _skillService = new SkillService(character, () => GetTier(), Refresh);
            _appearanceService = new AppearanceService(character, Refresh);
            _equipmentService = new EquipmentService(character, Refresh);
            _upgradeService = new TroopUpgradeService(character, Refresh);
            Vm = new TroopEditorVM(PreviewCharacter, this);
        }
        public string GetName() => _appearanceService.GetName();
        public bool GetIsFemale() => _appearanceService.GetIsFemale();
        public CultureObject GetCulture() => _appearanceService.GetCulture();
        public MBBodyProperty? GetBodyProperty() => _appearanceService.GetBodyProperty();
        public List<Equipment> GetBattleEquipments() => _equipmentService.GetBattleEquipments();
        public FormationClass GetDefaultGroup() => _equipmentService.GetDefaultGroup();
        public MBEquipmentRoster GetRoster() => _equipmentService.BuildRoster();
        public List<CharacterObject> GetUpgradeTargets() => _upgradeService.GetTroopUpgradeTargets();
        public int GetLevel() => _level;
        public int GetTier()
        {
            if (_level <= 1) return 0;
            return Math.Max(0, (_level - 1) / 5);
        }
        public void SetTier(int tier)
        {
            _level = TierToLevelMapper.GetLevelForTier(tier);
        }
        public int GetSkillValue(SkillObject skill) => _skillService.GetSkillValue(skill);
        public int GetCurrentSkillSum() => _skillService.GetCurrentSkillSum();
        public int GetSuggestedMaxSkillSum() => _skillService.GetSuggestedMaxSkillSum();
        public void Refresh()
        {
            _equipmentService.RecalculateDefaultGroup();
            Vm.RefreshValues();
        }
        public void UpdateSkill(SkillObject skill, int amount) => _skillService.UpdateSkill(skill, amount);
        public void Rename() => _appearanceService.Rename();
        public void ChangeCulture() => _appearanceService.ChangeCulture();
        public void ChangeGender() => _appearanceService.ToggleGender();
        public void SelectAppearance()
        {
            _appearanceService.OpenAppearanceSelector();
        }
        public void SelectUpgradeCharacter(CharacterObject previouscharacter)
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
            .Where(c => c.Occupation == Occupation.Soldier)
            .ToList();
            Action<CharacterObject> onSelect = (newCharacter =>
            {
                var slotToUpdate = Vm.UpgradeSlots.FirstOrDefault(slot => slot.Upgrade == previouscharacter);
                var slotWithNewCharacter = Vm.UpgradeSlots.Where(slot => slot.Upgrade == newCharacter);
                if (slotWithNewCharacter.Count() != 0) return;
                slotToUpdate.ChangeCharacter(newCharacter);
            });
            var controller = new ObjectSelectorController<CharacterObject>("Select Troop", allCharacters, onSelect, (character, close) => new CharacterCardVM(character, onSelect, close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        public void SelectItem(string slotKey) => _equipmentService.SelectItem(slotKey);
        public void FinalizeItem(EquipmentIndex index, ItemObject item) => _equipmentService.FinalizeItem(index, item);
        public void SetDefaultGroup() => _equipmentService.RecalculateDefaultGroup();
        public void AddUpgrade()
        {
            var currentUpgrades = _upgradeService.GetTroopUpgradeTargets();
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier)
                .Where(c => !currentUpgrades.Contains(c))
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>("Select Troop", allCharacters, _upgradeService.AddTroopUpgradeTarget, (character, close) => new CharacterCardVM(character, _upgradeService.AddTroopUpgradeTarget, close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        public void RemoveUpgrade(CharacterObject upgrade) => _upgradeService.RemoveTroopUpgradeTarget(upgrade);
        public void CopyTemplate()
        {
            var data = BuildTroopEditorData();
            var preview = CreatePreviewCharacter();
            EditorTemplateHolder.Instance.TroopTemplate = data;
            EditorTemplateHolder.Instance.TroopPreviewCharacter = preview;
            InformationManager.DisplayMessage(new InformationMessage("Troop template copied to clipboard"));
        }
        public TroopEditorData BuildTroopEditorData()
        {
            return new TroopEditorData
            {
                Name = GetName(),
                IsFemale = GetIsFemale(),
                DefaultGroup = GetDefaultGroup(),
                Tier = GetTier(),
                Culture = GetCulture(),
                UpgradesTo = GetUpgradeTargets(),
                Roster = GetRoster(),
                FaceKeyTemplate = GetBodyProperty(),
                SkillValues = BuildSkillValues()
            };
        }
        public void OpenTierInquiry()
        {
            List<InquiryElement> list = new();
            for (int i = 1; i <= 6; i++) list.Add(new InquiryElement(i, "Tier " + i, null));
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Tier", "", list, true, 1, 1, "Continue", null, args =>
            {
                InformationManager.HideInquiry();
                SetTier((int)args.First().Identifier);
                Refresh();
            }, null, "", false), false, false);
        }
        public void AddSet()
        {
            _equipmentService.AddSet();
            Refresh();
        }
        public void RemoveSet()
        {
            List<Equipment> battle = _equipmentService.GetBattleEquipments();
            if (battle.Count <= 1) return;
            List<InquiryElement> elements = new();
            for (int i = 0; i < battle.Count; i++)
            {
                Equipment equipment = battle[i];
                string weapon = equipment[EquipmentIndex.Weapon0].Item?.Name?.ToString() ?? "Empty";
                elements.Add(new InquiryElement(i, "Set " + (i + 1) + " : " + weapon, null));
            }
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select sets to remove", "", elements, true, 1, battle.Count - 1, "Continue", null, OnRemoveSetsConfirmed, null, "", false), false, false);
        }
        void OnRemoveSetsConfirmed(List<InquiryElement> args)
        {
            if (args == null || !args.Any()) return;
            InformationManager.HideInquiry();
            List<int> indicesToRemove = args.Select(e => (int)e.Identifier).OrderByDescending(i => i).ToList();
            _equipmentService.RemoveSets(indicesToRemove);
            Refresh();
        }
        public string FillFormationString()
        {
            var text = "Formation: ";
            var formationEnum = _equipmentService.GetDefaultGroup();
            var formationType = formationEnum switch
            {
                FormationClass.Infantry => "Infantry",
                FormationClass.Ranged => "Ranged",
                FormationClass.Cavalry => "Cavalry",
                FormationClass.HorseArcher => "Horse Archer",
                _ => "Infantry",
            };
            return text + formationType;
        }

        public MBBindingList<LanceBannerItemVM> GetLanceBannersForTroop(Action<string> onLanceClicked)
        {
            var banners = new MBBindingList<LanceBannerItemVM>();
            var lances = LanceTemplateManager.Instance.GetLancesForTroop(PreviewCharacter.StringId);
            foreach (var lance in lances)
                banners.Add(new LanceBannerItemVM(lance, onLanceClicked));
            return banners;
        }

        public void SwitchTroop()
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier)
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>("Switch Troop", allCharacters, OnSwitchTroop, (character, close) => new CharacterCardVM(character, OnSwitchTroop, close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        void OnSwitchTroop(CharacterObject selectedCharacter)
        {
            TroopEditorViewService.Delete();
            var newController = new TroopEditorController(selectedCharacter);
            TroopEditorViewService.Create(newController.Vm);
        }
        public void CreateNewTroop()
        {
            CharacterObject looter = MBObjectManager.Instance.GetObject<CharacterObject>("looter");
            CharacterObject newTroop = CharacterObject.CreateFrom(looter);
            var setNameMethod = AccessTools.Method("TaleWorlds.Core.BasicCharacterObject:SetName");
            setNameMethod.Invoke(newTroop, new object[] { GetUniqueTroopName("NewTroop") });
            var controller = new TroopEditorController(newTroop);
            TroopEditorViewService.Delete();
            TroopEditorViewService.Create(controller.Vm);
        }
        TextObject GetUniqueTroopName(string baseName)
        {
            if (MBObjectManager.Instance.GetObject<CharacterObject>(baseName) == null) return new(baseName);
            for (int i = 1; ; i++)
            {
                string name = baseName + "_" + i;
                if (MBObjectManager.Instance.GetObject<CharacterObject>(name) == null) return new(name);
            }
        }
        public void Update(bool refresh = true)
        {
            if (refresh) Refresh();
            else _equipmentService.RecalculateDefaultGroup();
        }
        public void OnSave()
        {
            DynamicTroopsService.Instance.SaveCharacterFromData(
                GetName(), GetIsFemale(), GetDefaultGroup(), GetTier(), GetCulture(),
                GetUpgradeTargets(), GetRoster(), GetBodyProperty(), BuildSkillValues());
            InformationManager.DisplayMessage(new InformationMessage("Saved: " + GetName()));
        }
        Dictionary<SkillObject, int> BuildSkillValues()
        {
            return new Dictionary<SkillObject, int>
            {
                { DefaultSkills.OneHanded, GetSkillValue(DefaultSkills.OneHanded) },
                { DefaultSkills.TwoHanded, GetSkillValue(DefaultSkills.TwoHanded) },
                { DefaultSkills.Polearm, GetSkillValue(DefaultSkills.Polearm) },
                { DefaultSkills.Bow, GetSkillValue(DefaultSkills.Bow) },
                { DefaultSkills.Crossbow, GetSkillValue(DefaultSkills.Crossbow) },
                { DefaultSkills.Throwing, GetSkillValue(DefaultSkills.Throwing) },
                { DefaultSkills.Riding, GetSkillValue(DefaultSkills.Riding) },
                { DefaultSkills.Athletics, GetSkillValue(DefaultSkills.Athletics) }
            };
        }
        public CharacterObject CreatePreviewCharacter()
        {
            CharacterObject preview = PreviewCharacter;
            var setNameMethod = AccessTools.Method("TaleWorlds.Core.BasicCharacterObject:SetName");
            setNameMethod.Invoke(preview, new object[] { new TextObject(GetName()) });
            preview.IsFemale = GetIsFemale();
            typeof(BasicCharacterObject).GetProperty("Culture")?.SetValue(preview, GetCulture());
            AccessTools.Property(typeof(BasicCharacterObject), "Level").SetValue(preview, GetLevel());
            typeof(CharacterObject).GetProperty("BodyPropertyRange")?.SetValue(preview, GetBodyProperty());
            MBCharacterSkills skills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(preview.StringId);
            skills.Skills.SetPropertyValue(DefaultSkills.OneHanded, GetSkillValue(DefaultSkills.OneHanded));
            skills.Skills.SetPropertyValue(DefaultSkills.TwoHanded, GetSkillValue(DefaultSkills.TwoHanded));
            skills.Skills.SetPropertyValue(DefaultSkills.Polearm, GetSkillValue(DefaultSkills.Polearm));
            skills.Skills.SetPropertyValue(DefaultSkills.Bow, GetSkillValue(DefaultSkills.Bow));
            skills.Skills.SetPropertyValue(DefaultSkills.Crossbow, GetSkillValue(DefaultSkills.Crossbow));
            skills.Skills.SetPropertyValue(DefaultSkills.Throwing, GetSkillValue(DefaultSkills.Throwing));
            skills.Skills.SetPropertyValue(DefaultSkills.Riding, GetSkillValue(DefaultSkills.Riding));
            skills.Skills.SetPropertyValue(DefaultSkills.Athletics, GetSkillValue(DefaultSkills.Athletics));
            typeof(CharacterObject).GetField("DefaultCharacterSkills")?.SetValue(preview, skills);
            MBEquipmentRoster roster = GetRoster();
            typeof(BasicCharacterObject).GetField("_equipmentRoster")?.SetValue(preview, roster);
            preview.InitializeEquipmentsOnLoad(preview);
            return preview;
        }
        public static EquipmentIndex ToItemSlot(string equipment) => EquipmentService.ToItemSlot(equipment);
    }
}
