using LanceSystem.Deserialization;
using LanceSystem.DynamicTroops.UI;
using LanceSystem.DynamicTroops.UI.ItemSelection;
using LanceSystem.DynamicTroops.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;
using static LanceSystem.DynamicTroops.UI.ItemSelection.Filters.FilterFactory;

namespace LanceSystem.DynamicLances.UI
{
    public class LanceTemplateEditorController
    {

        GauntletLayer? _layer;
        GauntletMovieIdentifier? _movie;
        SpriteCategory? _orderCategory;
        Lance _currentLance;
        Lance? _originalSnapshot;
        public LanceTemplateEditorVM Vm { get; internal set; }

        LanceTemplateEditorController(Lance lance)
        {
            _currentLance = lance;
            Vm = new LanceTemplateEditorVM(this, lance);
            CaptureSnapshot();
        }
        public static void CreateLayer(string lanceId)
        {
            var lance = LanceTemplateManager.Instance.GetLanceFromId(lanceId);
            var controller = new LanceTemplateEditorController(lance);
            controller.OpenLayer();
        }
        public static void CreateLayer()
        {
            var troopTemplates = new LanceTroopsTemplate(new() { new(LanceTroopCategory.Infantry, 0.5, "looter") });
            var lance = new Lance("temp", "Temp", null, null, LanceTemplateOriginType.All, troopTemplates);
            var controller = new LanceTemplateEditorController(lance);
            controller.OpenLayer();
        }


        void OpenLayer()
        {
            _orderCategory = UIResourceManager.LoadSpriteCategory("ui_order");
            _layer = new GauntletLayer("LanceTemplateEditorLayer", 1001);
            Vm.RefreshValues();
            _movie = _layer.LoadMovie("LanceTemplateEditor", Vm);
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            ScreenManager.TopScreen.AddLayer(_layer);
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
        }

        public void DeleteLayer()
        {
            var top = ScreenManager.TopScreen;
            if (_layer != null)
            {
                _layer.InputRestrictions.ResetInputRestrictions();
                _layer.IsFocusLayer = false;
                if (_movie != null) _layer.ReleaseMovie(_movie);
                top.RemoveLayer(_layer);
            }
            _orderCategory?.Unload();
            _orderCategory = null;
            _layer = null;
            _movie = null;
        }
        public void EditBanner(string bannerCode, Action<string?> onBannerEdited)
        {
            var clan = Clan.PlayerClan;
            var originalBanner = clan.Banner;
            var color1 = clan.Color;
            var color2 = clan.Color2;
            var editBanner = new Banner(clan.Banner.BannerCode);
            clan.Banner = editBanner;
            Game.Current.GameStateManager.PushState(
                Game.Current.GameStateManager.CreateState<BannerEditorState>(() =>
                {
                    var result = clan.Banner.BannerCode;
                    clan.Banner = originalBanner;
                    clan.Color = color1;
                    clan.Color2 = color2;
                    onBannerEdited(result != Clan.PlayerClan.Banner.BannerCode ? result : null);
                }));
        }

        public void ReactivateLayer()
        {
            if (_layer != null)
            {
                _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
                _layer.IsFocusLayer = true;
                ScreenManager.TrySetFocus(_layer);
            }
        }

        public void AddTroop()
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier)
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>("Select Troop", allCharacters, OnAddTroopSelected, (character, close) => new CharacterCardVM(character, OnAddTroopSelected, close), CreateCharacterFilters());
            controller.Open();
        }

        void OnAddTroopSelected(CharacterObject character)
        {
            var category = FormationClassToLanceTroopCategory(character.DefaultFormationClass);
            Vm.AddTroopRow(new TroopData(category, 0, character.StringId));
        }

        public void SelectTroopForRow(TroopDataRowVM row, int formationClassValue)
        {
            var category = IntToLanceTroopCategory(formationClassValue);
            var matchingFormationClasses = GetFormationClassesForCategory(category);
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier && matchingFormationClasses.Contains(c.DefaultFormationClass))
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>("Select Troop", allCharacters, c => OnTroopSelectedForRow(row, c), (character, close) => new CharacterCardVM(character, c => OnTroopSelectedForRow(row, c), close), CreateCharacterFilters());
            controller.Open();
        }

        void OnTroopSelectedForRow(TroopDataRowVM row, CharacterObject character)
        {
            row.BasicTroopId = character.StringId;
            var category = FormationClassToLanceTroopCategory(character.DefaultFormationClass);
            row.FormationClassValue = row.GetFormationClassValueFromCategory(category);
        }

        public void ChangeCategoryForRow(TroopDataRowVM row)
        {
            var elements = new List<InquiryElement>
            {
                new((int)LanceTroopCategory.Infantry, "Infantry", null),
                new((int)LanceTroopCategory.Ranged, "Ranged", null),
                new((int)LanceTroopCategory.Cavalry, "Cavalry", null),
                new((int)LanceTroopCategory.HorseArcher, "HorseArcher", null)
            };
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Change Category", "Select category", elements, true, 1, 1, "Confirm", null, args =>
            {
                if (args == null || !args.Any()) return;
                var selected = (LanceTroopCategory)(int)args.First().Identifier;
                row.FormationClassValue = row.GetFormationClassValueFromCategory(selected);
            }, null, "", false), false, false);
        }

        static LanceTroopCategory FormationClassToLanceTroopCategory(FormationClass formationClass)
        {
            return formationClass switch
            {
                FormationClass.Ranged or FormationClass.Skirmisher => LanceTroopCategory.Ranged,
                FormationClass.Cavalry or FormationClass.HeavyCavalry or FormationClass.LightCavalry => LanceTroopCategory.Cavalry,
                FormationClass.HorseArcher => LanceTroopCategory.HorseArcher,
                _ => LanceTroopCategory.Infantry,
            };
        }

        static LanceTroopCategory IntToLanceTroopCategory(int value)
        {
            return value switch
            {
                1 => LanceTroopCategory.Ranged,
                2 => LanceTroopCategory.Cavalry,
                3 => LanceTroopCategory.HorseArcher,
                _ => LanceTroopCategory.Infantry,
            };
        }

        static FormationClass[] GetFormationClassesForCategory(LanceTroopCategory category)
        {
            return category switch
            {
                LanceTroopCategory.Ranged => new[] { FormationClass.Ranged, FormationClass.Skirmisher },
                LanceTroopCategory.Cavalry => new[] { FormationClass.Cavalry, FormationClass.HeavyCavalry, FormationClass.LightCavalry },
                LanceTroopCategory.HorseArcher => new[] { FormationClass.HorseArcher },
                _ => new[] { FormationClass.Infantry },
            };
        }

        public void SwitchTemplate()
        {
            var lances = DynamicLancesService.Instance.GetAllLances().ToList();
            if (lances.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage("No lance templates available"));
                return;
            }
            var elements = lances.Select(l => new InquiryElement(l.StringId, l.Name, null)).ToList();
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Switch Template", "Select template to load", elements, true, 1, 1, "Load", null, args =>
            {
                if (args == null || !args.Any()) return;
                var id = (string)args.First().Identifier;
                var lance = DynamicLancesService.Instance.GetLance(id);
                if (lance == null) return;
                Vm.LoadFromLance(lance);
                _currentLance = lance;
                CaptureSnapshot();
            }, null, "", false), false, false);
        }

        public Lance BuildLance()
        {
            var troops = Vm.TroopRows.Select(r => r.ToTroopData()).ToList();
            var normalized = LanceDataDeserializer.NormalizeTroopLikelihoods(troops);
            string stringId = Vm.TemplateName.ToLower().Replace(" ", "_");
            return new Lance(stringId, Vm.TemplateName, null, null, LanceTemplateOriginType.All, new LanceTroopsTemplate(normalized), 1, string.IsNullOrWhiteSpace(Vm.BannerCode) ? null : Vm.BannerCode);
        }

        void CaptureSnapshot()
        {
            try { _originalSnapshot = BuildLance(); } catch { }
        }

        public void Save()
        {
            var lance = BuildLance();
            DynamicLancesService.Instance.SaveLance(lance.Name, lance.CultureId, lance.ClanId, lance.LanceOriginType, lance.TroopsTemplate, lance.weight, lance.bannerKey);
            _currentLance = lance;
            CaptureSnapshot();
            Vm.ClearDirty();
            Vm.RefreshLikelihoodSum();
            InformationManager.DisplayMessage(new InformationMessage("Saved: " + lance.Name));
        }

        public void CopyTemplate()
        {
            var lance = BuildLance();
            EditorTemplateHolder.Instance.LanceTemplate = lance;
            InformationManager.DisplayMessage(new InformationMessage("Lance template copied to clipboard"));
        }

        public void PasteTemplate()
        {
            var template = EditorTemplateHolder.Instance.LanceTemplate;
            if (template == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("No lance template in clipboard. Copy a template first."));
                return;
            }
            if (Vm.HasUnsavedChanges)
            {
                InformationManager.ShowInquiry(new InquiryData("Unsaved Changes", "There are unsaved changes. Do you want to discard them and paste the template?", true, true, "Yes", "No", () => ApplyPaste(template), null, "", 0f, null, null, null), true, false);
                return;
            }
            ApplyPaste(template);
        }

        void ApplyPaste(Lance template)
        {
            Vm.LoadFromLance(template);
            _currentLance = template;
            CaptureSnapshot();
            InformationManager.DisplayMessage(new InformationMessage("Lance template pasted"));
        }

        public void CreateNewTemplate()
        {
            var troopTemplates = new LanceTroopsTemplate(new() { new(LanceTroopCategory.Infantry, 0.5, "looter"), new(LanceTroopCategory.Ranged, 0.5, "looter") });
            var lance = new Lance("temp", "New Lance", null, null, LanceTemplateOriginType.All, troopTemplates);
            Vm.LoadFromLance(lance);
            _currentLance = lance;
            CaptureSnapshot();
        }

        public void OpenTroopEditor(string troopId)
        {
            if (string.IsNullOrWhiteSpace(troopId)) { InformationManager.DisplayMessage(new InformationMessage("TroopId empty")); return; }
            void DoOpen()
            {
                DeleteLayer();
                CharacterObject character = MBObjectManager.Instance.GetObject<CharacterObject>(troopId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>(troopId);
                if (character == null) return;
                var controller = new TroopEditorController(character);
                TroopEditorViewService.Create(controller.Vm);
            }
            if (Vm.HasUnsavedChanges)
                InformationManager.ShowInquiry(new InquiryData("Unsaved Changes", "There are unsaved changes, do you want to discard them and continue?", true, true, "Yes", "No", () => DoOpen(), null, "", 0f, null, null, null), true, false);
            else DoOpen();
        }

        public void SaveAndClose()
        {
            Save();
            DeleteLayer();
        }
    }
}
